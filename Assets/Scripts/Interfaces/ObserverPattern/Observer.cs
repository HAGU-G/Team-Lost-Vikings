public class Observer<T> : IObserver<T>
{
    public event System.Action OnNotified;
    public T LastSubject { get; private set; }
    public NOTIFY_TYPE LastNotifyType { get; private set; }

    public void ReceiveNotification(T subject, NOTIFY_TYPE type = NOTIFY_TYPE.NONE)
    {
        LastSubject = subject;
        LastNotifyType = type;
        OnNotified?.Invoke();
    }
}
