public interface IObserver<T>
{
    public void ReceiveNotification(T subject);
}