using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

public class UINotification : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.NOTIFICATION;

    public Button confirm;
    public Button exit;

    public void OnButtonConfirm()
    {

    }

    public void OnButtonExit()
    {
        Close();
    }
}
