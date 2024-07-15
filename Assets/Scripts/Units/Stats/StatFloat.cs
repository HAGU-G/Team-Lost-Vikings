using System;

[Serializable]
public class StatFloat : IFormattable
{
    public StatFloat(float defaultValue = default)
    {
        this.defaultValue = defaultValue;
    }

    public float defaultValue;
    public StatFloat upgradeValue = null;
    public float Current =>
        defaultValue + ((upgradeValue != null) ? (float)upgradeValue.Current : 0f);

    //Methods
    public override string ToString() => Current.ToString();
    public string ToString(IFormatProvider provider) => Current.ToString(provider);
    public string ToString(string format) => Current.ToString(format);
    public string ToString(string format, IFormatProvider provider) => Current.ToString(format, provider);

    public StatFloat Clone()
    {
        var clone = new StatFloat();
        clone.defaultValue = defaultValue;
        clone.upgradeValue = upgradeValue;

        return clone;
    }
}