using System.Collections.Generic;
using UnityEngine;

public class SaveDataV2 : SaveData
{
    public SaveDataV2() : base(2) { }

    public UnitManager unitManager;
    public PlayerManager playerManager;
    public ItemManager itemManager;
    public QuestManager questManager;
    public DialogManager dialogManager;
    public List<int> dialogQueue = new();

    public List<HuntZoneInfo> huntZones = new();
    public Dictionary<int, List<int>> UnitDeployment;

    public Dictionary<Vector2Int, int> buildings = new();
    public Dictionary<int, int> buildingUpgrade = new();
    public Dictionary<int, bool> buildingFlip = new();

    public override SaveData VersionDown()
    {
        SaveDataV1 v1 = new SaveDataV1();
        v1.unitManager = unitManager;
        v1.playerManager = playerManager;
        v1.itemManager = itemManager;
        v1.questManager = questManager;
        v1.dialogManager = dialogManager;

        v1.huntZones = huntZones;
        v1.UnitDeployment = UnitDeployment;
        v1.buildings = buildings;
        v1.buildingUpgrade = buildingUpgrade;
        v1.buildingFlip = buildingFlip;
        return v1;
    }

    public override SaveData VersionUp()
    {
        SaveDataV3 v3 = new();
        //V2
        v3.unitManager = unitManager;
        v3.playerManager = playerManager;
        v3.itemManager = itemManager;
        v3.questManager = questManager;
        v3.dialogManager = dialogManager;
        v3.dialogQueue = dialogQueue;

        v3.huntZones = huntZones;
        v3.UnitDeployment = UnitDeployment;
        v3.buildings = buildings;
        v3.buildingUpgrade = buildingUpgrade;
        v3.buildingFlip = buildingFlip;
        return this;
    }
}
