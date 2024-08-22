using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class UIWindowMessage : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.MESSAGE_POPUP;

    [Header("텍스트")]
    public TextMeshProUGUI textMessage;
    public TextMeshProUGUI textButtonLeft;
    public TextMeshProUGUI textButtonRight;

    [Header("버튼")]
    public GameObject buttonLayout;
    public Button buttonLeft;
    public Button buttonRight;

    [Header("배경")]
    public Image imageBackground;
    public bool isBlockOther;

    [Header("애니메이션")]
    public Animator animator;
    public enum OPEN_ANIMATION
    {
        NONE,
        BLACKOUT,
        FADEOUT,
    }
    private OPEN_ANIMATION openAnimation;
    private static readonly int paramType = Animator.StringToHash("Type");
    private static readonly int paramIsOpended = Animator.StringToHash("IsOpened");

    [Header("기타")]
    public GameObject window;
    public bool isAutoClose;
    public float closeTime;
    private float closeTimer = 0f;

    //팝업 대기열
    public class MessageInfo
    {
        public string message;
        public bool isBlockOther;
        public float autoCloseTime;
        public Action onOpen;
        public Action onClose;
        public OPEN_ANIMATION openAnimation;
    }
    public Queue<MessageInfo> waitList = new();

    //이벤트
    private Action OnOpenOnce;
    private Action OnCloseOnce;

    //입력
    private InputManager im;
    private bool isPressed = false;
    private bool isShowButton = false;



    public override void Open()
    {
        base.Open();

        //배경 설정
        imageBackground.enabled = isBlockOther;
        imageBackground.raycastTarget = isBlockOther;

        OnOpenOnce?.Invoke();
        OnOpenOnce = null;

        //애니메이션
        if (openAnimation == OPEN_ANIMATION.BLACKOUT)
            imageBackground.enabled = true;

        animator.SetInteger(paramType, (int)openAnimation);
        animator.SetBool(paramIsOpended, isOpened);
    }

    protected override void OnGameStart()
    {
        base.OnGameStart();
        im = GameManager.inputManager;
    }

    public override void Close()
    {
        base.Close();

        OnCloseOnce?.Invoke();
        OnCloseOnce = null;
        OnOpenOnce = null;

        isPressed = false;

        animator.SetBool(paramIsOpended, isOpened);

        if (waitList.Count > 0)
        {
            var info = waitList.Dequeue();
            ShowMessage(
                info.message,
                info.isBlockOther,
                info.autoCloseTime,
                info.onOpen,
                info.onClose,
                info.openAnimation);
        }
    }

    public void SetMessage(string message)
    {
        textMessage.text = message;
    }


    /// <param name="message">표시할 메시지</param>
    /// <param name="isBlockOther">뒷 UI를 가리는 배경 출력</param>
    /// <param name="autoCloseTime">자동으로 닫히는 시간, 0이하의 값일 경우 자동으로 닫히지 않음</param>
    /// <param name="onOpen">Open될 때 실행할 Action, Open Close될 때 제거</param>
    /// <param name="onClose">Close될 때 실행할 Action, Close될 때 제거</param>
    /// <param name="openAnimation">Open될 때 적용할 애니메이션</param>
    public void ShowMessage(
        string message,
        bool isBlockOther,
        float autoCloseTime = 0f,
        Action onOpen = null,
        Action onClose = null,
        OPEN_ANIMATION openAnimation = OPEN_ANIMATION.NONE)
    {
        if (isOpened)
        {
            waitList.Enqueue(new()
            {
                message = message,
                isBlockOther = isBlockOther,
                autoCloseTime = autoCloseTime,
                onOpen = onOpen,
                onClose = onClose,
                openAnimation = openAnimation
            });
            return;
        }

        isAutoClose = autoCloseTime > 0f;
        closeTime = autoCloseTime;

        OnOpenOnce += onOpen;
        OnCloseOnce += onClose;

        this.isBlockOther = isBlockOther;
        SetMessage(message);

        this.openAnimation = openAnimation;

        Open();
    }

    private void Update()
    {
        if (isAutoClose)
        {
            closeTimer += Time.deltaTime;

            if (closeTimer >= closeTime)
            {
                closeTimer = 0f;
                Close();
            }
        }


        // 닫기 처리
        if (isShowButton || im == null || isAutoClose)
            return;

        isPressed |= im.Pressed;
        if (isPressed && im.Up)
            Close();
    }
}