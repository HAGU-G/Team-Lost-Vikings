using System.Collections.Generic;
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
    public static TableDuplicatedID<int, SkillPool> skillPoolTable = new("Assets/DataTable/SkillPoolTable.csv");
    public static Table<int, StatsData> unitTable = new("Assets/DataTable/UnitTable.csv");

    public static Table<int, HuntZoneData> huntZoneTable = new("Assets/DataTable/HuntZoneTable.csv");
    public static Table<int, DropData> dropTable = new("Assets/DataTable/DropTable.csv");

    public static Table<int, BuildingData> buildingTable = new("Assets/DataTable/BuildingTable.csv");
    public static TableDuplicatedID<int, UpgradeData> upgradeTable = new("Assets/DataTable/UpgradeTable.csv");

    public static Table<int, AchievementData> achievementTable = new("Assets/DataTable/AchievementTable.csv");
    public static TableDuplicatedID<int, DialogData> dialogTable = new("Assets/DataTable/DialogTable.csv");
    public static Table<int, QuestData> questTable = new("Assets/DataTable/QuestTable.csv");

    public static Table<int, TileData> tileTable = new("Assets/DataTable/TownMapTable.csv");
    //public static Table<int, TileData> huntZoneMapTable_1 = new("Assets/DataTable/HuntZone1MapTable.csv");
    //public static Table<int, TileData> huntZoneMapTable_2 = new("Assets/DataTable/HuntZone2MapTable.csv");
    //public static Table<int, TileData> huntZoneMapTable_3 = new("Assets/DataTable/HuntZone3MapTable.csv");

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

            tileTable.Load(),
            //huntZoneMapTable_1.Load(),
            //huntZoneMapTable_2.Load(),
            //huntZoneMapTable_3.Load(),
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
