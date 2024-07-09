public interface ISubject<T>
{
    public void Subscribe(IObserver<T> observer);
    public void UnSubscrive(IObserver<T> observer);
}