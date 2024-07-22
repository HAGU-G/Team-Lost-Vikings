using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

public class UIBuildingParameterPopUp : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.PARAMETER_POPUP;

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
