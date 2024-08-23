using UnityEngine.UI;

//TESTCODE

public class UICharacterManagement : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.CHARACTER_MANAGEMENT;

    public Button buttonOK;
    public Button buttonCancel;
    public InputField inputStageID;


    public void OnButtonExit()
    {
        GameManager.PlayButtonSFX();
        Close();
    }
}