using UnityEngine;

public abstract class IState<T> : MonoBehaviour
{
    protected T controller;
    public virtual void Init(T controller)
    {
        this.controller = controller;
    }
    public virtual void ResetState()
    { 
        enabled = false;
    }
    public virtual void EnterState() 
    { 
        enabled = true;
    }
    public virtual void ExitState() 
    {
        enabled = false;
    }
    public virtual bool CanExit() { return true; }
}