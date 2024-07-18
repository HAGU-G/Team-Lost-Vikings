using System;

[Serializable]
public class StatInt : StatClass<int>
{
    protected override int Add(int left, int right)
    {
        return left + right;
    }

    public override StatClass<int> Clone()
    {
        var clone = new StatInt();
        clone.defaultValue = defaultValue;
        clone.upgradeValue = upgradeValue;

        return clone;
    }
}