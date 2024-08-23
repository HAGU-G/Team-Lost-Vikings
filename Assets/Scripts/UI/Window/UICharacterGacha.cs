using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
public class UICharacterGacha : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.GACHA_UI;

    public ItemManager im;

    public Button gacha;
    public Button exit;
    public TextMeshProUGUI requireGoldText;
    public TextMeshProUGUI autoGachaText;

    private int requireGold = 1000;
    private bool isOpen = false;

    protected override void Awake()
    {
        base.Awake();
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

    public void SetGachaUI()
    {
        requireGoldText.text = $"{im.Gold} / {requireGold}";

        bool isEnough = true;

        if (im.Gold >= requireGold)
        {
            gacha.targetGraphic.color = Color.green;
            requireGoldText.color = Color.black;
        }
        else
        {
            gacha.targetGraphic.color = Color.gray;
            requireGoldText.color = Color.red;
            isEnough = false;
        }
        gacha.interactable = isEnough && GameManager.unitManager.CanGacha;

        //모집소 건물이 없을 시 가챠되지 않도록 설정

    }

    public void OnButtonGacha()
    {
        var result = GameManager.unitManager.GachaCharacter(GameManager.playerManager.recruitLevel);
        im.Gold -= requireGold;
        SetGachaUI();
        //가챠 연출, 결과창 보여주는 건 아래 메서드 마지막 부분으로 옮김.
        PlayGachaAnimation(result);
    }

    public void OnButtonExit()
    {
        isOpen = false;
        Close();
    }

    private void PlayGachaAnimation(UnitStats result)
    {
        if (result == null)
            return;

        var cm = GameManager.cameraManager;
        var vm = GameManager.villageManager;
        ///////////////////////////
        ////////// 시작 ///////////
        ///////////////////////////

        // TODO 레터박스 제외 UI 숨기기 필요
        Close();

        // 카메라 상태 저장
        GameManager.inputManager.receiver.enabled = false;

        var cameraFocusing = cm.isFocusOnUnit;
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
        cm.FinishFocousOnUnit();
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

        // 달려와서 정지
        animator.listener.OnGachaWaitEventOnce += () =>
        {
            animator.AnimIdle();
            var screenEffect = GameManager.effectManager.GetEffect("WhiteScreen");
            screenEffect.transform.position = centerPos;
        };

        // 결과 보여주기
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

                // TODO UI 숨기기 해제 필요


                // 연출용 캐릭터 삭제
                Addressables.ReleaseInstance(gachaPrefab);
                Addressables.ReleaseInstance(resultCharacter);

                // 카메라 상태 되돌리기
                cm.isHideUnits = isHideUnits;
                cm.SetLocation(cameraLocation, cameraHuntZoneNum);
                cm.SetPosition(cameraPositin);
                cm.ZoomValue = cameraZoom;
                Camera.main.orthographicSize = cameraZoom;
                GameManager.inputManager.receiver.enabled = true;

                //결과 표시
                var uiResult = GameManager.uiManager.windows[WINDOW_NAME.GACHA_RESULT] as UIGachaResult;
                uiResult.SetResult(result);
                uiResult.Open();
            };
    }
}
