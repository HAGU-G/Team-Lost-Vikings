using System;
using UnityEngine;

public abstract class StatClass<T>
{
    public static readonly string debugFormat = "defaultValue : {0}\nupgradeValue : {1}\nCurrent : {2}";
    protected static T Zero { get; set; }

    public StatClass(T defaultValue)
    {
        this.defaultValue = defaultValue;
    }
    public StatClass() : this(Zero) { }

    public event Action OnDefaultValueChanged;

    public T _defaultValue;
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

    public T Current => Add(defaultValue, ((upgradeValue != null) ? upgradeValue.Current : Zero));

    protected abstract T Add(T left, T right);

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

#if !UNITY_EDITOR
        upgradeValue = target;
    }
#else
        if (upgradeValue != null)
            upgradeValue.OnDefaultValueChanged -= UpdateUpOnInspector;

        upgradeValue = target;
        target.OnDefaultValueChanged += UpdateUpOnInspector;
        UpdateUpOnInspector();
    }

    public void UpdateUpOnInspector()
    {
        upOnInspector = upgradeValue.defaultValue;
    }

    public T upOnInspector;

#endif
}