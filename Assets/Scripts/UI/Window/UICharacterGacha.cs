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
            gacha.enabled = true;
            requireGoldText.color = Color.black;

        }
        else
        {
            ColorBlock colorBlock = gacha.colors;
            colorBlock.normalColor = Color.gray;
            gacha.colors = colorBlock;
            gacha.enabled = false;
            requireGoldText.color = Color.red;
        }
    }

    public void OnButtonGacha()
    {
        GameManager.unitManager.GachaCharacter(GameManager.playerManager.level);
        var gachaResultUnit = GameManager.unitManager.Waitings.Last();
        im.Gold -= requireGold;
        
        GameManager.uiManager.windows[WINDOW_NAME.GACHA_RESULT].Open();
    }

    public void OnButtonExit()
    {
        Close();

    }
}
