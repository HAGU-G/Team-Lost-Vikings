using System.Collections.Generic;
using UnityEngine;

public class SaveDataV3 : SaveData
{
    public SaveDataV3() : base(3) { }

    //V2
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

    //V3
    public float masterVolume = 1f;
    public float bgmVolume = 1f;
    public float sfxVolume = 1f;
    public int frameRate = 60;

    public override SaveData VersionDown()
    {
        SaveDataV2 v2 = new SaveDataV2();
        //V2
        v2.unitManager = unitManager;
        v2.playerManager = playerManager;
        v2.itemManager = itemManager;
        v2.questManager = questManager;
        v2.dialogManager = dialogManager;
        v2.dialogQueue = dialogQueue;

        v2.huntZones = huntZones;
        v2.UnitDeployment = UnitDeployment;
        v2.buildings = buildings;
        v2.buildingUpgrade = buildingUpgrade;
        v2.buildingFlip = buildingFlip;
        return v2;
    }

    public override SaveData VersionUp()
    {
        SaveDataV4 v4 = new SaveDataV4();

        //V2
        v4.unitManager = unitManager;
        v4.playerManager = playerManager;
        v4.itemManager = itemManager;
        v4.questManager = questManager;
        v4.dialogManager = dialogManager;
        v4.dialogQueue = dialogQueue;

        v4.huntZones = huntZones;
        v4.UnitDeployment = UnitDeployment;
        v4.buildings = buildings;
        v4.buildingUpgrade = buildingUpgrade;
        v4.buildingFlip = buildingFlip;

        //V3
        v4.masterVolume = masterVolume;
        v4.bgmVolume = bgmVolume;
        v4.sfxVolume = sfxVolume;
        v4.frameRate = frameRate;

        foreach(var unit in v4.unitManager.Units.Values)
        {
            unit.Buffs.Clear();
        }
        foreach (var unit in v4.unitManager.Waitings.Values)
        {
            unit.Buffs.Clear();
        }
        foreach (var unit in v4.unitManager.DeadUnits.Values)
        {
            unit.Buffs.Clear();
        }

        return v4;
    }
}
