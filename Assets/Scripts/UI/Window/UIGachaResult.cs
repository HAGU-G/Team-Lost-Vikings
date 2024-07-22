using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

public class UIGachaResult : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.GACHA_RESULT;

    public Button exit;

    public void OnButtonExit()
    {
        gameObject.SetActive(false);
    }
}
