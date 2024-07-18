using UnityEngine;

public abstract class IWindowable : MonoBehaviour
{
    public bool isShowOnly;

    private void Awake()
    {
        GameManager.uiManager.AddWindow(this);
    }

    public virtual void Open()
    {
        if(isShowOnly)
            GameManager.uiManager.CloseWindows(this);
    }

    public virtual void Close()
    {

    }
}
