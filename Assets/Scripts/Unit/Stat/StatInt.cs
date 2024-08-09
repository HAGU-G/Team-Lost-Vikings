using System;
using UnityEngine;

[Serializable]
public class StatInt : StatClass<int>
{
    protected override int Add(params int[] values)
    {
        int result = 0;
        foreach (var value in values)
        {
            result += value;
        }
        return result;
    }

    public override StatClass<int> Clone()
    {
        var clone = new StatInt();
        clone.defaultValue = defaultValue;
        clone.upgradeValue = upgradeValue;

        return clone;
    }

    protected override int Multiply(params int[] values)
    {
        int result = 1;
        foreach (var value in values)
        {
            result += value;
        }
        return result;
    }

    protected override int Multiply(float left, int right)
    {
        return Mathf.FloorToInt(left * right);
    }
}