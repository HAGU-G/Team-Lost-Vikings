using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

public class UIUnitDetailInformation : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.UNIT_DETAIL_INFORMATION;

    public Button placement;
    public Button exit;

    public void OnButtonPlacement()
    {

    }

    public void OnButtonExit()
    {
        gameObject.SetActive(false);
    }
}