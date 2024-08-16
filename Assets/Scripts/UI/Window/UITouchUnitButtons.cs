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

        close.onClick.AddListener(OnButtonClose);
    }

    private void OnButtonClose()
    {
        GameManager.cameraManager.FinishFocousOnUnit();
        Close();
    }
}