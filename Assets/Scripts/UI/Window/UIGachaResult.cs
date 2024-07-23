using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class UIGachaResult : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.GACHA_RESULT;

    public TextMeshProUGUI gachaResultDesc;

    public Image unitImage;
    public TextMeshProUGUI unitGrade;
    public TextMeshProUGUI unitName;

    public Button exit;
    public UnitStats gachaResultUnit;

    private void OnEnable()
    {
        gachaResultUnit = GameManager.unitManager.Waitings.Last().Value;
        SetResult();
    }

    public void SetResult()
    {
        gachaResultDesc.text = $"{gachaResultUnit.UnitGrade.ToString()} 등급의 {gachaResultUnit.Name}를 뽑았습니다.";
        //unitImage.sprite = GameManager.uiManager.unitRenderTexture.LoadRenderTexture(gachaResultUnit.AssetFileName);
        unitGrade.text = gachaResultUnit.UnitGrade.ToString();
        unitName.text = gachaResultUnit.Name;
    }

    public void OnButtonExit()
    {
        gameObject.SetActive(false);
    }
}
