﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using static UIWindowMessage;
public class UICharacterGacha : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.GACHA_UI;

    public ItemManager im;

    public Button gacha;
    public Button exit;
    public TextMeshProUGUI requireGoldText;
    public TextMeshProUGUI autoGachaText;

    private int requireRune = 1000;
    private bool isOpen = false;

    protected override void Awake()
    {
        base.Awake();
        isShowOnly = false;
    }

    private void Update()
    {
        SetGachaTimeText(GameManager.unitManager.TimeToAutoGacha);
    }

    private void SetGachaTimeText(float time)
    {
        autoGachaText.text = $"자동 모집 남은 시간 : {time:0.0}초 남음";
    }

    protected override void OnGameStart()
    {
        base.OnGameStart();
        im = GameManager.itemManager;

        GameManager.Subscribe(EVENT_TYPE.CONFIGURE, OnGameConfigure);
    }


    private void OnEnable()
    {
        if (!IsReady)
            return;

        isOpen = true;

        SetGachaUI();
    }

    private void OnGameConfigure()
    {
        im.OnItemChangedCallback += OnItemChanged;
    }

    private void OnItemChanged()
    {
        if (isOpen)
            SetGachaUI();
    }

    public bool SetGachaUI()
    {
        int runeAmount = im.GetItem(GameSetting.Instance.runeID);
        requireGoldText.text = $"{runeAmount} / {requireRune}";

        bool isEnough = true;

        if (runeAmount >= requireRune)
        {
            gacha.targetGraphic.color = Color.green;
            requireGoldText.color = Color.white;
        }
        else
        {
            gacha.targetGraphic.color = Color.gray;
            requireGoldText.color = Color.red;
            isEnough = false;
        }

        //gacha.interactable = isEnough && GameManager.unitManager.CanGacha;
        return isEnough;
    }

    public void OnButtonGacha()
    {
        GameManager.PlayButtonSFX();

        if (!CheckGacha())
            return;

        var result = GameManager.unitManager.GachaCharacter(GameManager.playerManager.recruitLevel);
        im.SpendItem(GameSetting.Instance.runeID, requireRune);
        SetGachaUI();
        //가챠 연출, 결과창 보여주는 건 아래 메서드 마지막 부분으로 옮김.
        PlayGachaAnimation(result);
    }

    private bool CheckGacha()
    {
        var vm = GameManager.villageManager;
        var message = GameManager.uiManager.windows[WINDOW_NAME.MESSAGE_POPUP] as UIWindowMessage;
        if (im.GetItem(GameSetting.Instance.runeID) < requireRune)
        {
            message.ShowMessage(
                $"모집에 필요한 재화가 부족합니다.",
                true,
                1.5f,
                openAnimation: UIWindowMessage.OPEN_ANIMATION.FADEINOUT,
                closeType: CLOSE_TYPE.TOUCH);

            return false;
        }
        else if (!vm.constructedBuildings.Contains(vm.GetBuilding(STRUCTURE_ID.RECRUIT)))
        {
            message.ShowMessage(
                $"모집소가 없어 모험가를 모집을 할 수 없습니다.\n모집소 건물을 건설해주세요.",
                true,
                1.5f,
                openAnimation: UIWindowMessage.OPEN_ANIMATION.FADEINOUT,
                closeType: CLOSE_TYPE.TOUCH);
            return false;
        }
        else if (!GameManager.unitManager.CanGacha)
        {
            if (GameManager.unitManager.IsMaxWait)
                message.ShowMessage(
                $"대기소에 빈 공간이 없어 모집을 진행할 수 없습니다.\n대기소를 비워주세요",
                true,
                1.5f,
                openAnimation: UIWindowMessage.OPEN_ANIMATION.FADEINOUT,
                closeType: CLOSE_TYPE.TOUCH);
            else if (GameManager.unitManager.GetGachaPool(GameManager.playerManager.recruitLevel).Count <= 0)
                message.ShowMessage(
               $"모집할 수 있는 모든 모험가를 모집했습니다.\n모집소 건물을 업그레이드해주세요.",
               true,
               1.5f,
               openAnimation: UIWindowMessage.OPEN_ANIMATION.FADEINOUT,
               closeType: CLOSE_TYPE.TOUCH);

            return false;
        }

        return true;
    }

    public void OnButtonExit()
    {
        GameManager.PlayButtonSFX();
        isOpen = false;
        Close();
    }

    private void PlayGachaAnimation(UnitStats result)
    {
        if (result == null)
            return;

        ///////////////////////////
        ////////// 시작 ///////////
        ///////////////////////////

        GameManager.PlayAnimation();
        var cm = GameManager.cameraManager;
        var vm = GameManager.villageManager;

        foreach (var canvas in GameManager.uiManager.uiDevelop.canvases)
        {
            canvas.gameObject.SetActive(false);
        }

        Close();

        // 카메라 상태 저장
        GameManager.inputManager.receiver.enabled = false;

        var isFocusing = cm.isFocusOnUnit;
        var focusingUnit = cm.focusingUnit;
        var cameraPositin = cm.transform.position;
        var cameraLocation = cm.LookLocation;
        var cameraHuntZoneNum = cm.HuntZoneNum;
        var cameraZoom = cm.ZoomValue;
        var isHideUnits = cm.isHideUnits;



        ///////////////////////////
        ////////// 연출 ///////////
        ///////////////////////////

        // 카메라 세팅
        cm.isHideUnits = true;
        cm.FinishFocusOnUnit();
        cm.SetLocation(LOCATION.VILLAGE);
        cm.Zoom(7.4f);
        var standardBuilding = vm.GetBuilding(STRUCTURE_ID.STANDARD);
        Vector3 centerPos = Vector3.zero;
        if (standardBuilding != null)
        {
            var index = vm.gridMap.PosToIndex(standardBuilding.transform.position);
            index -= new Vector2Int(1, 1);
            centerPos = vm.gridMap.IndexToPos(index);
            cm.SetPosition(centerPos);
        }

        string prefabName = result.UnitGrade switch
        {
            UNIT_GRADE.NORMAL => GameSetting.Instance.gachaPrefabName,
            UNIT_GRADE.COMMON => GameSetting.Instance.gachaPrefabName,
            UNIT_GRADE.RARE => GameSetting.Instance.gachaPrefabName,
            UNIT_GRADE.SUPER_RARE => GameSetting.Instance.gachaPrefabRareName,
            UNIT_GRADE.ULTRA_RARE => GameSetting.Instance.gachaPrefabRareName,
            _ => GameSetting.Instance.gachaPrefabName
        };

        // 연출용 프리펩 생성
        DressAnimator animator = new();
        var gachaPrefab = Addressables.InstantiateAsync(
            prefabName,
            centerPos,
            Quaternion.identity).WaitForCompletion();
        gachaPrefab.transform.localScale = Vector3.one * 2f;
        animator.Init(gachaPrefab.GetComponentInChildren<Animator>(), new() { defaultValue = 3f }, new() { defaultValue = 1f });
        gachaPrefab.gameObject.AddComponent<SortingGroup>().sortingLayerName = SORT_LAYER.OverUnit.ToString();


        // 애니메이션 재생
        animator.AnimRun();
        animator.AnimGacha();

        // 결과 프리펩 생성
        var resultCharacter = Addressables.InstantiateAsync(
            result.Data.UnitAssetFileName,
            centerPos,
            Quaternion.identity).WaitForCompletion();

        resultCharacter.transform.localScale = Vector3.one * 2f;
        resultCharacter.SetActive(false);
        resultCharacter.gameObject.AddComponent<SortingGroup>().sortingLayerName = SORT_LAYER.OverUnit.ToString();


        // 달려와서 정지
        animator.listener.OnGachaWaitEventOnce += () =>
        {
            animator.AnimIdle();
            var screenEffect = GameManager.effectManager.GetEffect("WhiteScreen");
            screenEffect.transform.position = centerPos;
        };

        // 결과 프리펩 보여주기
        animator.listener.OnGachaShowEventOnce += () =>
        {
            animator.SetAlpha(0f);
            resultCharacter.SetActive(true);
        };

        //종료
        animator.listener.OnGachaEndEventOnce += () =>
            {
                ///////////////////////////
                ////////// 종료 ///////////
                ///////////////////////////

                foreach (var canvas in GameManager.uiManager.uiDevelop.canvases)
                {
                    canvas.gameObject.SetActive(true);
                }

                // 연출용 캐릭터 삭제
                Addressables.ReleaseInstance(gachaPrefab);
                Addressables.ReleaseInstance(resultCharacter);

                // 카메라 상태 되돌리기
                GameManager.inputManager.receiver.enabled = true;
                cm.isHideUnits = isHideUnits;
                cm.SetLocation(cameraLocation, cameraHuntZoneNum);
                cm.SetPosition(cameraPositin);
                cm.isFocusOnUnit = isFocusing;

                if (!isFocusing)
                {
                    cm.ZoomValue = cameraZoom;
                    Camera.main.orthographicSize = cameraZoom;
                }
                else
                {
                    GameManager.uiManager.windows[WINDOW_NAME.TOUCH_UNIT_BUTTONS].Open();
                }
                //결과 표시
                var uiResult = GameManager.uiManager.windows[WINDOW_NAME.GACHA_RESULT] as UIGachaResult;
                uiResult.SetResult(result);
                uiResult.Open();

                GameManager.StopAnimation();
            };
    }
}
