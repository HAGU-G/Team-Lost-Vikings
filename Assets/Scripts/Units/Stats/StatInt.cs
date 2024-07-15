using System;

[Serializable]
public class StatInt : IFormattable
{
    public StatInt(int defaultValue = default)
    {
        this.defaultValue = defaultValue;
    }

    public int defaultValue;
    public StatInt upgradeValue = null;
    public int Current =>
       defaultValue + ((upgradeValue != null) ? upgradeValue.Current : 0);

    //Methods
    public override string ToString() => Current.ToString();
    public string ToString(IFormatProvider provider) => Current.ToString(provider);
    public string ToString(string format) => Current.ToString(format);
    public string ToString(string format, IFormatProvider provider) => Current.ToString(format, provider);

    public StatInt Clone()
    {
        var clone = new StatInt();
        clone.defaultValue = defaultValue;
        clone.upgradeValue = upgradeValue;

        return clone;
    }
}