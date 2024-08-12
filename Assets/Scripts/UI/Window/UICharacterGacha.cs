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

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnGameStart()
    {
        base.OnGameStart();
        im = GameManager.itemManager;
    }


    private void OnEnable()
    {
        if (!IsReady)
            return;

        SetGachaUI();
    }

    public void SetGachaUI()
    {
        requireGoldText.text = $"{im.Gold} / {requireGold}";

        //To-Do : 가챠 요구 골드 조건 넣기  
        if (im.Gold >= requireGold)
        {
            ColorBlock colorBlock = gacha.colors;
            colorBlock.normalColor = Color.green;
            gacha.colors = colorBlock;
            gacha.interactable = true;
            requireGoldText.color = Color.black;

        }
        else
        {
            ColorBlock colorBlock = gacha.colors;
            colorBlock.normalColor = Color.gray;
            gacha.colors = colorBlock;
            gacha.interactable = false;
            requireGoldText.color = Color.red;
        }

        if (GameManager.unitManager.unitLimitCount <= GameManager.unitManager.Units.Count)
            gacha.interactable = false;
    }

    public void OnButtonGacha()
    {
        var result = GameManager.unitManager.GachaCharacter(GameManager.playerManager.level);
        im.Gold -= requireGold;
        SetGachaUI();
        var uiResult = GameManager.uiManager.windows[WINDOW_NAME.GACHA_RESULT] as UIGachaResult;
        uiResult.SetResult(result);
        uiResult.Open();
    }

    public void OnButtonExit()
    {
        Close();

    }
}
