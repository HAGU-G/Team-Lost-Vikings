using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildingPopUp : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.BUILDING_POPUP;

    public Button upgrade;
    public Button exit;

    public void OnButtonUpgrade()
    {
        GameManager.villageManager.village.Upgrade();
    }

    public void OnButtonExit()
    {
        gameObject.SetActive(false);
    }

}
