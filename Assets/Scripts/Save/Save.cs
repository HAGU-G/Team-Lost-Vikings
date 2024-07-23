using System.Collections.Generic;

public abstract class SaveData
{
    public readonly int version;
    public SaveData(int version)
    {
        this.version = version;
    }

    public abstract SaveData VersionUp();
    public abstract SaveData VersionDown();
}

public class SaveDataV1 : SaveData
{
    public SaveDataV1() : base(1) { }

    public List<UnitStats> units;



    public override SaveData VersionDown()
    {
        return this;
    }

    public override SaveData VersionUp()
    {
        return this;
    }
}
