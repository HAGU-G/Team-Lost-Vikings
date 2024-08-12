using System.Linq;
using TMPro;
using UnityEngine.UI;

public class UIGachaResult : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.GACHA_RESULT;

    public TextMeshProUGUI gachaResultDesc;
    public Image gradeICon;
    public RawImage unitImage;
    public TextMeshProUGUI unitName;

    public Button exit;
    //public UnitStats gachaResultUnit;

    private void OnEnable()
    {
        if (!IsReady)
            return;
    }

    public void SetResult(UnitStats unitStats)
    {
        gachaResultDesc.text = $"{unitStats.UnitGrade.ToString()} 등급의 {unitStats.Data.Name}를 뽑았습니다.";
        //unitImage.sprite = GameManager.uiManager.unitRenderTexture.LoadRenderTexture(gachaResultUnit.AssetFileName);
        unitImage.uvRect = GameManager.uiManager.unitRenderTexture.LoadRenderTexture(unitStats.Data.UnitAssetFileName);
        gradeICon.sprite = GameManager.uiManager.gradeIcons[(int)unitStats.UnitGrade];
        unitName.text = unitStats.Data.Name;
    }

    public void OnButtonExit()
    {
        Close();
    }
}
