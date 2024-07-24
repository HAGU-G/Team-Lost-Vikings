using UnityEngine.UI;

public class UITutorialPopUp : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.TUTORIAL_POPUP;

    public Button exit;
    public Button previous;
    public Button next;

    public void OnButtonExit()
    {
        Close();
    }

    public void OnButtonPrevious()
    {

    }

    public void OnButtonNext()
    {

    }
}