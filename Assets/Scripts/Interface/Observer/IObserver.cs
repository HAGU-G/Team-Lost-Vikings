public enum NOTIFY_TYPE
{
    NONE,
    DEAD,
    REMOVE
}


public interface IObserver<T>
{
    public void ReceiveNotification(T subject, NOTIFY_TYPE type = NOTIFY_TYPE.NONE);
}