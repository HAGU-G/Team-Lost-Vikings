using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

public class UICharacterGacha : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.GACHA_UI;

    public Button gacha;
    public Button exit;

    public void OnButtonGacha()
    {

    }

    public void OnButtonExit()
    {
        gameObject.SetActive(false);
    }
}
