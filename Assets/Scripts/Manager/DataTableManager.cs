﻿using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum TABLE_NAME
{
    UNIT,
    SKILL,

}

public static class DataTableManager
{
    public static Table<int, ItemData> itemTable = new("Assets/DataTable/ItemTable.csv");
    public static Table<int, PlayerData> playerTable = new("Assets/DataTable/LevelTable.csv"); 

    public static Table<int, SkillData> skillTable = new("Assets/DataTable/SkillTable.csv");
    public static Table<int, SkillPool> skillPoolTable = new("Assets/DataTable/SkillPoolTable.csv");
    public static Table<int, StatsData> unitTable = new("Assets/DataTable/UnitTable.csv");

    public static Table<int, HuntZoneData> huntZoneTable = new("Assets/DataTable/HuntZoneTable.csv");
    public static Table<int, DropData> dropTable = new("Assets/DataTable/DropTable.csv");

    public static Table<int, BuildingData> buildingTable = new("Assets/DataTable/BuildingTable.csv");
    public static TableDuplicatedID<int, UpgradeData> upgradeTable = new("Assets/DataTable/UpgradeTable.csv");

    public static Table<int, AchievementData> achievementTable = new("Assets/DataTable/AchievementTable.csv");
    public static Table<int, DialogData> dialogTable = new("Assets/DataTable/DialogTable.csv");
    public static Table<int, Quest> questTable = new("Assets/DataTable/QuestTable.csv");

    public static List<AsyncOperationHandle> handles;
    public static float progress = 0f;
    public static bool IsReady = false;

    public static void Load()
    {
        handles = new()
        {
            itemTable.Load(),
            playerTable.Load(),

            skillTable.Load(),
            skillPoolTable.Load(),
            unitTable.Load(),

            huntZoneTable.Load(),
            dropTable.Load(),

            buildingTable.Load(),
            upgradeTable.Load(),

            achievementTable.Load(),
            dialogTable.Load(),
            questTable.Load(),
        };
    }

    public static void Update()
    {
        progress = 0f;
        IsReady = true;

        foreach (var handle in handles)
        {
            progress += handle.PercentComplete;
            IsReady &= handle.IsDone;
        }

        progress /= handles.Count;
    }
}
