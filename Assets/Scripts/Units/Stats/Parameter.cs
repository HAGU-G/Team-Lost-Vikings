using System;

[Serializable]
public class Parameter : IComparable<Parameter>, IEquatable<Parameter>, IFormattable
{
    public Parameter(int defaultValue = 0)
    {
        this.defaultValue = defaultValue;
    }

    public int defaultValue {get; private set;}
    public bool hasMax = false;
    public int min = 0;
    public StatInt max = new();

    private int _current;
    public int Current
    {
        get => _current;
        set
        {
            _current = value;

            if (_current < min)
            {
                _current = min;
                return;
            }
            if (hasMax && max != null && _current > (int)max)
            {
                _current = (int)max;
                return;
            }
        }
    }

    public float Ratio
    {
        get
        {
            if (!hasMax)
                return 1f;
            else if (max == null)
                return 1f;

            return Current - min / (int)max;
        }
    }

    //operator
    public static explicit operator int(Parameter self) => self.Current;
    public static explicit operator float(Parameter self) => self.Current;

    #region COMPARE
    public static bool operator >(Parameter left, int right) => left.Current > right;
    public static bool operator <(Parameter left, int right) => left.Current < right;
    public static bool operator >=(Parameter left, int right) => left.Current >= right;
    public static bool operator <=(Parameter left, int right) => left.Current <= right;
    public static bool operator ==(Parameter left, int right) => left.Current == right;
    public static bool operator !=(Parameter left, int right) => left.Current != right;
    public static bool operator >(int left, Parameter right) => left > right.Current;
    public static bool operator <(int left, Parameter right) => left < right.Current;
    public static bool operator >=(int left, Parameter right) => left >= right.Current;
    public static bool operator <=(int left, Parameter right) => left <= right.Current;
    public static bool operator ==(int left, Parameter right) => left == right.Current;
    public static bool operator !=(int left, Parameter right) => left != right.Current;
    public static bool operator >(Parameter left, StatInt right) => left.Current > right.Current;
    public static bool operator <(Parameter left, StatInt right) => left.Current < right.Current;
    public static bool operator >=(Parameter left, StatInt right) => left.Current >= right.Current;
    public static bool operator <=(Parameter left, StatInt right) => left.Current <= right.Current;
    public static bool operator ==(Parameter left, StatInt right) => left.Current == right.Current;
    public static bool operator !=(Parameter left, StatInt right) => left.Current != right.Current;
    public static bool operator >(StatInt left, Parameter right) => left.Current > right.Current;
    public static bool operator <(StatInt left, Parameter right) => left.Current < right.Current;
    public static bool operator >=(StatInt left, Parameter right) => left.Current >= right.Current;
    public static bool operator <=(StatInt left, Parameter right) => left.Current <= right.Current;
    public static bool operator ==(StatInt left, Parameter right) => left.Current == right.Current;
    public static bool operator !=(StatInt left, Parameter right) => left.Current != right.Current;
    #endregion

    //Methods
    public int CompareTo(Parameter other)
    {
        return Current.CompareTo(other.Current);
    }
    public bool Equals(Parameter other)
    {
        return Current.Equals(other.Current);
    }
    public string ToString(string format, IFormatProvider formatProvider)
    {
        return Current.ToString(format, formatProvider);
    }
    public void Reset()
    {
        Current = defaultValue;
    }
    public void SetMin(int min)
    {
        this.min = min;
    }
    public void SetMax(StatInt max)
    {
        this.max = max;
        hasMax = true;
    }
}
