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

    protected abstract bool Transition();
}