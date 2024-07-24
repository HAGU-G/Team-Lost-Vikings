using UnityEngine;

public abstract class UIWindow : MonoBehaviour
{
    public bool isShowOnly = true;
    public abstract WINDOW_NAME WindowName { get; }
    protected bool IsReady = false;

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
        GameManager.uiManager.isWindowOn = true;
        gameObject.SetActive(true);
        if (isShowOnly)
            GameManager.uiManager.CloseWindows(this);
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);

        foreach (var window in GameManager.uiManager.windows)
        {
            if (window.Value.gameObject.activeSelf)
                return;
        }
        GameManager.uiManager.isWindowOn = false;
    }
}
