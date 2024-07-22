using System.Collections.Generic;
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
    public static Table<int, MonsterStatsData> monsterTable = new("Assets/DataTable/MonsterTable.csv");
    public static Table<int, SkillData> skillTable = new("Assets/DataTable/SkillTable.csv");
    public static Table<int, SkillPool> skillPoolTable = new("Assets/DataTable/SkillPoolTable.csv");

    public static List<AsyncOperationHandle> handles = new();
    public static float progress = 0f;
    public static bool IsReady = false;

    public static void Load()
    {
        handles.Add(characterTable.Load());
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
