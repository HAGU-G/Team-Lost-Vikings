using TMPro;
using UnityEngine.UI;

public class UINotification : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.NOTIFICATION;

    public Button confirm;
    public Button exit;
    public RawImage unitImage;
    public TextMeshProUGUI description;

    public void OnButtonConfirm()
    {

    }

    public void OnButtonExit()
    {
        Close();
    }
}
