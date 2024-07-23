﻿using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum TABLE_NAME
{
    UNIT,
    SKILL,

}

public static class DataTableManager
{
    public static Table<int, UnitStatsData> characterTable = new("Assets/DataTable/UnitTable.csv");
    public static Table<int, SkillData> skillTable = new("Assets/DataTable/SkillTable.csv");
    public static Table<int, SkillPool> skillPoolTable = new("Assets/DataTable/SkillPoolTable.csv");

    public static Table<int, HuntZoneData> huntZoneTable = new("Assets/DataTable/HuntZoneTable.csv");
    public static Table<int, MonsterStatsData> monsterTable = new("Assets/DataTable/MonsterTable.csv");
    public static Table<int, DropData> dropTable = new("Assets/DataTable/DropTable.csv");

    public static Table<int, BuildingData> buildingTable = new("Assets/DataTable/BuildingTable.csv");
    public static TableDuplicatedID<int, UpgradeData> upgradeTable = new("Assets/DataTable/UpgradeTable.csv");

    public static List<AsyncOperationHandle> handles;
    public static float progress = 0f;
    public static bool IsReady = false;

    public static void Load()
    {
        handles = new()
        {
            characterTable.Load(),
            skillTable.Load(),
            skillPoolTable.Load(),

            monsterTable.Load(),
            huntZoneTable.Load(),
            dropTable.Load(),

            buildingTable.Load(),
            upgradeTable.Load(),
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
