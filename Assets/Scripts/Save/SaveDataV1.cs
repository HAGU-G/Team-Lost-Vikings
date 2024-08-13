using System.Collections.Generic;

public class SaveDataV1 : SaveData
{
    public SaveDataV1() : base(1) { }

    public UnitManager unitManager;
    public PlayerManager playerManager;
    public ItemManager itemManager;
    public QuestManager questManager;
    public DialogManager dialogManager;

    public List<HuntZoneInfo> huntZones = new();
    public Dictionary<int, List<int>> UnitDeployment;
    public List<int> buildingUpgrade = new();

    public override SaveData VersionDown()
    {
        return this;
    }

    public override SaveData VersionUp()
    {
        return this;
    }
}
