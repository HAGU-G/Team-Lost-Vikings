/// <summary>
/// T는 Key의 데이터형입니다.
/// </summary>
/// <typeparam name="T">Key</typeparam>
public interface ITableAvaialable<T>
{
    public T TableID { get; }
}