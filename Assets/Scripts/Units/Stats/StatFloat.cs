using System;

[Serializable]
public class StatFloat : IComparable<StatFloat>, IEquatable<StatFloat>, IFormattable
{
    public StatFloat(float defaultValue = default)
    {
        this.defaultValue = defaultValue;
    }

    public float defaultValue;
    public StatFloat upgradeValue = null;
    public float Value => defaultValue
        + ((upgradeValue != null) ? (float)upgradeValue : 0f);

    //operator
    public static explicit operator int(StatFloat self) => (int)self.Value;
    public static explicit operator float(StatFloat self) => self.Value;

    //Methods
    public int CompareTo(StatFloat other)
    {
        return Value.CompareTo(other.Value);
    }
    public bool Equals(StatFloat other)
    {
        return Value.Equals(other.Value);
    }
    public string ToString(string format, IFormatProvider formatProvider)
    {
        return Value.ToString(format, formatProvider);
    }
}