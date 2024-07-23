using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

public class UITutorialPopUp : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.TUTORIAL_POPUP;

    public Button exit;
    public Button previous;
    public Button next;

    public void OnButtonExit()
    {
        gameObject.SetActive(false);
    }

    public void OnButtonPrevious()
    {

    }

    public void OnButtonNext()
    {

    }
}