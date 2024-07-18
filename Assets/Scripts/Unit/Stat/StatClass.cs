public abstract class StatClass<T>
{
    public static readonly string debugFormat = "defaultValue : {0}\nupgradeValue : {1}\nCurrent : {2}";
    protected static T Zero { get; set; }

    public StatClass(T defaultValue)
    {
        this.defaultValue = defaultValue;
    }
    public StatClass() : this(Zero) { }

    public T defaultValue;
    public StatClass<T> upgradeValue = null;

    public T Current => Add(defaultValue, ((upgradeValue != null) ? upgradeValue.Current : Zero));

    protected abstract T Add(T left, T right);

    public override string ToString()
    {
        var result = string.Format(
            debugFormat,
            defaultValue,
            (upgradeValue == null) ? 0 : upgradeValue,
            Current);
        return result;
    }
    public abstract StatClass<T> Clone();
}