using System;

[Serializable]
public class StatFloat : StatClass<float>
{
    protected override float Add(params float[] values)
    {
        float result = 0;
        foreach (var value in values)
        {
            result += value;
        }
        return result;
    }

    public override StatClass<float> Clone()
    {
        var clone = new StatFloat();
        clone.defaultValue = defaultValue;
        clone.upgradeValue = upgradeValue;

        return clone;
    }

    protected override float Multiply(params float[] values)
    {
        float result = 1f;
        foreach (var value in values)
        {
            result *= value;
        }
        return result;
    }

    protected override float Multiply(float left, float right)
    {
        return left * right;
    }
}