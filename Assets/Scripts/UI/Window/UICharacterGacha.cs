using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterGacha : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.GACHA_UI;

    public ItemManager im;

    public Button gacha;
    public Button exit;
    public TextMeshProUGUI requireGoldText;

    private int requireGold = 1000;
    private bool isOpen = false;

    protected override void Awake()
    {
        base.Awake();
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
        gacha.interactable = isEnough;

        if (GameManager.unitManager.unitLimitCount <= GameManager.unitManager.Units.Count)
            gacha.interactable = false;

        //모집소 건물이 없을 시 가챠되지 않도록 설정

    }

    public void OnButtonGacha()
    {
        var result = GameManager.unitManager.GachaCharacter(GameManager.playerManager.recruitLevel);
        im.Gold -= requireGold;
        SetGachaUI();
        var uiResult = GameManager.uiManager.windows[WINDOW_NAME.GACHA_RESULT] as UIGachaResult;
        uiResult.SetResult(result);
        uiResult.Open();
    }

    public void OnButtonExit()
    {
        isOpen = false;
        Close();
    }
}
