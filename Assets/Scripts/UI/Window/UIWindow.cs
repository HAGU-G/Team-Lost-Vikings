using UnityEngine;

public abstract class UIWindow : MonoBehaviour
{
    public bool isShowOnly = true;
    public abstract WINDOW_NAME WindowName { get; }

    private void Awake()
    {
        GameManager.uiManager.AddWindow(WindowName, this);
    }

    public virtual void Open()
    {
        gameObject.SetActive(true);
        if (isShowOnly)
            GameManager.uiManager.CloseWindows(this);
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }
}
