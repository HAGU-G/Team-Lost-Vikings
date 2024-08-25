using System.Collections.Generic;
using UnityEngine;

public class SaveDataV4 : SaveData
{
    public SaveDataV4() : base(4) { }

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
        SaveDataV3 v3 = new SaveDataV3();
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

        //V3
        v3.masterVolume = masterVolume;
        v3.bgmVolume = bgmVolume;
        v3.sfxVolume = sfxVolume;
        v3.frameRate = frameRate;

        return v3;
    }

    public override SaveData VersionUp()
    {
        return this;
    }
}
