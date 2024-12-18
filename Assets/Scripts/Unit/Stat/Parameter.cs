﻿using Newtonsoft.Json;
using System;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class Parameter : IFormattable
{
    public int min = 0;
    public int max = int.MaxValue;


    [JsonProperty]
    [field: UnityEngine.SerializeField]
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
            if (_current > max)
            {
                _current = max;
                return;
            }
        }
    }

    public float Ratio
    {
        get
        {
            if (max == 0)
                return 1f;

            return (Current - min) / (float)max;
        }
    }
    //Methods
    public override string ToString() => Current.ToString();
    public string ToString(IFormatProvider provider) => Current.ToString(provider);
    public string ToString(string format) => Current.ToString(format);
    public string ToString(string format, IFormatProvider provider) => Current.ToString(format, provider);
    public void Reset()
    {
        Current = max;
    }

    public Parameter Clone()
    {
        var clone = new Parameter();
        clone.min = min;
        clone.max = max;
        clone.Current = Current;

        return clone;
    }
}
