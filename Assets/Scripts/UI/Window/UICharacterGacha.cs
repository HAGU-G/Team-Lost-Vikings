using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    private void Awake()
    {
        im = GameManager.itemManager;
    }

    private void OnEnable()
    {
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

        
        GameManager.uiManager.windows[(int)WINDOW_NAME.GACHA_RESULT].Open();
    }

    public void OnButtonExit()
    {
        gameObject.SetActive(false);
    }
}
