using UnityEngine.UI;

public class UITouchUnitButtons : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.TOUCH_UNIT_BUTTONS;

    public Button information;
    public Button close;
    public Button placement;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnGameStart()
    {
        base.OnGameStart();
        isShowOnly = false;

        information.onClick.AddListener(OnButtonInformation);
        close.onClick.AddListener(OnButtonClose);
        placement.onClick.AddListener(OnButtonPlacement);
    }

    private void OnButtonInformation()
    {
       GameManager.uiManager.windows[WINDOW_NAME.UNIT_DETAIL_INFORMATION].Open();
    }

    private void OnButtonClose()
    {
        GameManager.cameraManager.FinishFocousOnUnit();
        Close();
    }

    private void OnButtonPlacement()
    {
        GameManager.uiManager.windows[WINDOW_NAME.CHARACTER_LOCATE].Open();
    }
}