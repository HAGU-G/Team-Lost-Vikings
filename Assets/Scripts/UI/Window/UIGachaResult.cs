﻿using System.Linq;
using TMPro;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

public class UIGachaResult : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.GACHA_RESULT;

    public TextMeshProUGUI gachaResultDesc;
    public Image gradeICon;
    public RawImage unitImage;
    public TextMeshProUGUI unitName;

    public Button exit;
    public UnitStats gachaResultUnit;

    private void OnEnable()
    {
        if (!IsReady)
            return;

        gachaResultUnit = GameManager.unitManager.Waitings.Last().Value;
        SetResult();
    }

    public void SetResult()
    {
        gachaResultDesc.text = $"{gachaResultUnit.UnitGrade.ToString()} 등급의 {gachaResultUnit.Name}를 뽑았습니다.";
        //unitImage.sprite = GameManager.uiManager.unitRenderTexture.LoadRenderTexture(gachaResultUnit.AssetFileName);
        unitImage.uvRect = GameManager.uiManager.unitRenderTexture.LoadRenderTexture(gachaResultUnit.AssetFileName);
        gradeICon.sprite = GameManager.uiManager.gradeIcons[(int)gachaResultUnit.UnitGrade];
        unitName.text = gachaResultUnit.Name;
    }

    public void OnButtonExit()
    {
        Close();
    }
}
