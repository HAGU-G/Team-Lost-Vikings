using System;

[Serializable]
public class StatFloat : StatClass<float>
{
    protected override float Add(float left, float right)
    {
        return left + right;
    }

    public override StatClass<float> Clone()
    {
        var clone = new StatFloat();
        clone.defaultValue = defaultValue;
        clone.upgradeValue = upgradeValue;

        return clone;
    }
}