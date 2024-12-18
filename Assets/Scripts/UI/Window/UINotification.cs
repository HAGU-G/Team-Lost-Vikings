﻿using TMPro;
using UnityEngine.UI;

public class UINotification : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.NOTIFICATION;

    public Button confirm;
    public Button exit;
    public RawImage unitImage;
    public TextMeshProUGUI description;
    private UnitStats unit;
    private string location;

    private void OnEnable()
    {
        unit = GameManager.uiManager.currentUnitStats;
        SetWindow();
    }

    private void SetWindow()
    {
        switch (unit.Location)
        {
            case LOCATION.VILLAGE:
                location = "마을";
                break;
            case LOCATION.HUNTZONE:
                location = $"{unit.HuntZoneNum}번 사냥터";
                break;
            default:
                location = "NONE";
                break;
        }
        unitImage.uvRect = GameManager.uiManager.unitRenderTexture.LoadRenderTexture(unit.Data.UnitAssetFileName);
        description.text = $"선택한 캐릭터가 현재 {location}에 있습니다.\n이동하시겠습니까?";
    }

    public void OnButtonConfirm()
    {
        Close();
        var cm = GameManager.cameraManager;
        GameManager.PlayButtonSFX();
        cm.SetLocation(unit.Location, unit.HuntZoneNum);
        cm.SetPosition(unit.objectTransform.position);
       //GameManager.cameraManager.SetViewPoint(unit);
    }

    public void OnButtonExit()
    {
        GameManager.PlayButtonSFX();
        Close();
    }

    //private void Update()
    //{
    //    //SetWindow();
    //}
}
