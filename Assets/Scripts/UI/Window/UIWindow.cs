using UnityEngine;

public abstract class UIWindow : MonoBehaviour
{
    public bool isShowOnly = true;
    public abstract WINDOW_NAME WindowName { get; }
    protected bool IsReady = false;
    public bool isOpened = false;

    protected virtual void Awake()
    {
        GameManager.Subscribe(EVENT_TYPE.START, OnGameStart);
        gameObject.SetActive(false);
    }

    protected virtual void OnGameStart()
    {
        GameManager.uiManager.AddWindow(WindowName, this);
        IsReady = true;
        Close();

    }

    public virtual void Open()
    {
        isOpened = true;
        GameManager.uiManager.isWindowOn = true;
        gameObject.SetActive(true);

        if (isShowOnly)
            GameManager.uiManager.CloseWindows(this);

        //if (GameManager.cameraManager.isFocousOnUnit)
        //    GameManager.cameraManager.FinishFocousOnUnit();

        GameManager.uiManager.OpenWindow(WindowName);
    }

    public virtual void Close()
    {
        isOpened = false;
        gameObject.SetActive(false);
        foreach (var window in GameManager.uiManager.windows)
        {
            if (window.Value.gameObject.activeSelf)
                return;
        }
        GameManager.uiManager.isWindowOn = false;
    }
}
