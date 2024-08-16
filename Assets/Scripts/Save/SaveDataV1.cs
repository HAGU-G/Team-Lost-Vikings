using System.Collections.Generic;
using UnityEngine;

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

    public Dictionary<Vector2Int, int> buildings = new();
    public Dictionary<int, int> buildingUpgrade = new();
    public Dictionary<int, bool> buildingFlip = new();

    public override SaveData VersionDown()
    {
        return this;
    }

    public override SaveData VersionUp()
    {
        return this;
    }
}
