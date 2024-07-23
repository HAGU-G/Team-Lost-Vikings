using System.Collections.Generic;

public class SaveDataV1 : SaveData
{
    public SaveDataV1() : base(1) { }

    public UnitManager unitManager;
    //public HuntZoneManager huntZoneManager;



    public override SaveData VersionDown()
    {
        return this;
    }

    public override SaveData VersionUp()
    {
        return this;
    }
}
