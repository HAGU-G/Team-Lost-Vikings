using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

public class UICharacterStash : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.CHARACTER_STASH;

    public Button exit;

    public void OnButtonExit()
    {
       gameObject.SetActive(false);
    }
}
