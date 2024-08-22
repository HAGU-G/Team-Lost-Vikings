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
    public TextMeshProUGUI textButtonConfirm;
    public TextMeshProUGUI textButtonCancel;
    private static readonly string stringConfirmDefault = "확인";
    private static readonly string stringCancelDefault = "취소";

    [Header("버튼")]
    public GameObject buttonLayout;
    public Button buttonConfirm;
    public Button buttonCancel;

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
        FADEINOUT,
    }

    public enum CLOSE_TYPE
    {
        /// <summary,상황에 맞춰 닫기</summary>
        AUTO,
        /// <summary>아무곳이나 터치했을 때 닫기</summary>
        TOUCH,
        /// <summary>버튼을 눌렀을 때 닫기</summary>
        BUTTON
    }

    private OPEN_ANIMATION openAnimation;
    private static readonly int paramType = Animator.StringToHash("Type");
    private static readonly int paramIsOpended = Animator.StringToHash("IsOpened");

    [Header("기타")]
    public GameObject window;
    public bool isAutoClose;
    public CLOSE_TYPE closeType;
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
        public CLOSE_TYPE closeType;
        public Action onConfirmButtonClick;
        public Action onCancelButtonClick;
        public string confirmButtonText;
        public string cancelButtonText;
    }
    public Queue<MessageInfo> waitList = new();

    //이벤트
    private Action OnOpenOnce;
    private Action OnCloseOnce;
    private Action OnConfirmButtonClickOnce;
    private Action OnCancelButtonClickOnce;

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

        isAutoClose = false;
        OnCloseOnce?.Invoke();
        OnCloseOnce = null;
        OnOpenOnce = null;
        OnCancelButtonClickOnce = null;
        OnConfirmButtonClickOnce = null;
        buttonLayout.SetActive(false);

        isPressed = false;

        animator.SetBool(paramIsOpended, isOpened);

        var dm = GameManager.dialogManager;

        if (GameManager.IsReady
            && !dm.IsShowing
            && dm.DialogQueue.Count > 0)
        {
            dm.Start(dm.DialogQueue.Peek());
        }

        if (waitList.Count > 0)
        {
            var info = waitList.Dequeue();
            ShowMessage(
                info.message,
                info.isBlockOther,
                info.autoCloseTime,
                info.onOpen,
                info.onClose,
                info.openAnimation,
                info.closeType,
                info.onConfirmButtonClick,
                info.onCancelButtonClick,
                info.confirmButtonText,
                info.cancelButtonText);
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
    /// <param name="openAnimation">Open될 때 적용할 애니메이션</param>
    /// <param name="onConfirmButtonClick">확인 버튼을 클릭할 때 실행 및 제거, null이 아닐경우 버튼 ON</param>
    /// <param name="onCancelButtonClick">취소 버튼을 클릭할 때 실행 및 제거, null이 아닐경우 버튼 ON</param>
    /// <param name="confirmButtonText">확인 버튼 텍스트</param>
    /// <param name="cancelButtonText">취소 버튼 텍스트</param>
    /// 버튼에 닫기 기능만 주고 활성화하려면 빈 람다식을 매개변수로 주면 됩니다.
    public void ShowMessage(
        string message,
        bool isBlockOther,
        float autoCloseTime = 0f,
        Action onOpen = null,
        Action onClose = null,
        OPEN_ANIMATION openAnimation = OPEN_ANIMATION.NONE,
        CLOSE_TYPE closeType = CLOSE_TYPE.AUTO,
        Action onConfirmButtonClick = null,
        Action onCancelButtonClick = null,
        string confirmButtonText = null,
        string cancelButtonText = null)
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
                openAnimation = openAnimation,
                closeType = closeType,
                onCancelButtonClick = onCancelButtonClick,
                onConfirmButtonClick = onConfirmButtonClick,
                confirmButtonText = confirmButtonText,
                cancelButtonText = cancelButtonText
            });
            return;
        }

        this.closeType = closeType;
        closeTime = autoCloseTime;

        OnCancelButtonClickOnce += onCancelButtonClick;
        OnConfirmButtonClickOnce += onConfirmButtonClick;
        OnOpenOnce += onOpen;
        OnCloseOnce += onClose;

        if (onCancelButtonClick != null)
        {
            isShowButton = true;
            buttonLayout.gameObject.SetActive(true);
            buttonCancel.gameObject.SetActive(true);
            textButtonCancel.text = cancelButtonText != null ? cancelButtonText : stringCancelDefault;
        }
        else
        {
            buttonCancel.gameObject.SetActive(false);
        }

        if (onConfirmButtonClick != null)
        {
            isShowButton = true;
            buttonLayout.gameObject.SetActive(true);
            buttonConfirm.gameObject.SetActive(true);
            textButtonConfirm.text = confirmButtonText != null ? confirmButtonText : stringConfirmDefault;
        }
        else
        {
            buttonConfirm.gameObject.SetActive(false);
        }


        if (autoCloseTime > 0f)
            isAutoClose = true;
        else if (closeType == CLOSE_TYPE.AUTO && isShowButton)
            closeType = CLOSE_TYPE.BUTTON;
        else
            closeType = CLOSE_TYPE.TOUCH;

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
                return;
            }
        }

        switch (closeType)
        {
            case CLOSE_TYPE.TOUCH:
                if (isShowButton || im == null)
                    return;
                isPressed |= im.Pressed;
                if (isPressed && im.Up)
                    Close();
                break;
            default:
                break;
        }
    }

    public void OnLeftButtonClick()
    {
        Close();
        OnConfirmButtonClickOnce?.Invoke();
        OnConfirmButtonClickOnce = null;
    }

    public void OnRightButtonClick()
    {
        Close();
        OnCancelButtonClickOnce?.Invoke();
        OnCancelButtonClickOnce = null;
    }
}