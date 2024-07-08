public abstract class State<T>
{
    protected T owner;
    protected FSM<T> controller;

    public virtual void Init(T owner, FSM<T> controller)
    {
        this.owner = owner;
        this.controller = controller;
    }

    public abstract void ResetState();

    public abstract void EnterState();

    public abstract void ExitState();

    public abstract void Update();

    /// <returns>상태가 전환 됐을 경우 true</returns>
    protected abstract bool Transition();
}