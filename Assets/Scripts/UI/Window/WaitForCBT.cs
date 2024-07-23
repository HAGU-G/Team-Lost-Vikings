using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

public class WaitForCBT : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.WAIT_FOR_CBT;

    public Button exit;

    public void OnButtonExit()
    {
        Close();
    }
}
