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
        SaveDataV2 v2 = new SaveDataV2();
        v2.unitManager = unitManager;
        v2.playerManager = playerManager;
        v2.itemManager = itemManager;
        v2.questManager = questManager;
        foreach (var quest in v2.questManager.GuideQuests)
        {
            v2.questManager.GuideQuests[quest.Key] = false;
        }
        v2.dialogManager = dialogManager;
        v2.dialogManager.ShowedDialog.Clear();

        v2.huntZones = huntZones;
        v2.UnitDeployment = UnitDeployment;
        v2.buildings = buildings;
        v2.buildingUpgrade = buildingUpgrade;
        v2.buildingFlip = buildingFlip;
        return v2;
    }
}
