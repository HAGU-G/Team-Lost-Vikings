using System;
using System.Runtime.CompilerServices;

[Serializable]
public class StatInt : IComparable<StatInt>, IEquatable<StatInt>, IFormattable
{
    public StatInt(int defaultValue = default)
    {
        this.defaultValue = defaultValue;
    }

    public int defaultValue;
    public StatInt upgradeValue = null;
    public int Current => defaultValue
        + ((upgradeValue != null) ? (int)upgradeValue : 0);

    //operator
    public static explicit operator int(StatInt self) => self.Current;
    public static explicit operator float(StatInt self) => self.Current;

    //Methods
    public int CompareTo(StatInt other)
    {
        return Current.CompareTo(other.Current);
    }
    public bool Equals(StatInt other)
    {
        return Current.Equals(other.Current);
    }
    public string ToString(string format, IFormatProvider formatProvider)
    {
        return Current.ToString(format, formatProvider);
    }
}