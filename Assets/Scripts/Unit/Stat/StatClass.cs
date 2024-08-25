using Newtonsoft.Json;
using System;

[JsonObject(MemberSerialization.OptIn)]
public abstract class StatClass<T>
{
    public static readonly string debugFormat = "defaultValue : {0}\nupgradeValue : {1}\nCurrent : {2}";

    public StatClass(T defaultValue)
    {
        this.defaultValue = defaultValue;
    }
    public StatClass() : this(default) { }

    public event Action OnDefaultValueChanged;

    [JsonProperty] public T _defaultValue;
    public T defaultValue
    {
        get
        {
            return _defaultValue;
        }
        set
        {
            _defaultValue = value;
            OnDefaultValueChanged?.Invoke();
        }
    }
    public StatClass<T> upgradeValue = null;
    [JsonProperty] public T buffValue;
    [JsonProperty] public float ratioBuffValue;

    public T Current =>
        Multiply(1f + ratioBuffValue,
            Add(defaultValue,
                (upgradeValue != null) ? upgradeValue.Current : default,
                buffValue));

    protected abstract T Add(params T[] values);
    protected abstract T Multiply(params T[] values);
    protected abstract T Multiply(float left, T right);

    public override string ToString()
    {
        var result = string.Format(
            debugFormat,
            defaultValue,
            (upgradeValue == null) ? 0 : upgradeValue.defaultValue,
            Current);
        return result;
    }
    public abstract StatClass<T> Clone();

    public void SetUpgrade(StatClass<T> target)
    {
        if (target == this || target == upgradeValue)
            return;

        upgradeValue = target;
    }
}