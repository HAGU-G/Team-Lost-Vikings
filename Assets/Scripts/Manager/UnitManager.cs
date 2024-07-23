using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

[JsonObject(MemberSerialization.OptIn)]
public class UnitManager
{
    /// <summary>
    /// Key: instanceID,
    /// Value: UnitStats,
    /// 꼭 필요한 경우가 아니라면 GetUnit() 메서드를 사용해주세요.
    /// </summary>
    [JsonProperty] public Dictionary<int, UnitStats> Units { get; private set; } = new();
    [JsonProperty] public Dictionary<int, UnitStats> DeadUnits { get; private set; } = new();
    [JsonProperty] public Dictionary<int, UnitStats> Waitings { get; private set; } = new();

    public void LoadUnits()
    {
        Debug.Log(UnitStats.existIDs.Count);
        Debug.Log(Units.Count);
        Debug.Log(DeadUnits.Count);
        Debug.Log(Waitings.Count);

        foreach (var unit in Units)
        {
            unit.Value.InitStats(DataTableManager.characterTable.GetData(unit.Value.Id), false);
            unit.Value.UpdateMaxHP();
            unit.Value.UpdateCombatPoint();
            unit.Value.SetUpgradeStats();
            if (!UnitStats.existIDs.Contains(unit.Value.InstanceID))
                Debug.Log("초비상");
        }
        foreach (var unit in DeadUnits)
        {
            unit.Value.InitStats(DataTableManager.characterTable.GetData(unit.Value.Id), false);
            unit.Value.UpdateMaxHP();
            unit.Value.UpdateCombatPoint();
            unit.Value.SetUpgradeStats();
        }
        foreach (var unit in Waitings)
        {
            unit.Value.InitStats(DataTableManager.characterTable.GetData(unit.Value.Id), false);
            unit.Value.UpdateMaxHP();
            unit.Value.UpdateCombatPoint();
            if (!UnitStats.existIDs.Contains(unit.Value.InstanceID))
                Debug.Log("초비상");
            //unit.Value.SetUpgradeStats(); 하면 안됨.
        }

        GameManager.Subscribe(EVENT_TYPE.START, OnGameStart);
    }

    private void OnGameStart()
    {
        foreach (var unit in Units)
        {
            SpawnOnLocation(unit.Value);
        }
    }


    public UnitStats GetUnit(int instanceID)
    {
        if (!Units.ContainsKey(instanceID))
        {
            Debug.LogWarning($"{instanceID} 존재하지 않는 유닛입니다.");
            return null;
        }

        return Units[instanceID];
    }

    private UnitStatsData GachaUnitData(int level)
    {
        var gachaList = new List<UnitStatsData>();

        foreach (var data in DataTableManager.characterTable.GetDatas())
        {
            if (level < data.GachaLevel)
                continue;

            for (int i = 0; i < data.GachaChance; i++)
            {
                gachaList.Add(data);
            }
        }

        return gachaList[Random.Range(0, gachaList.Count)];
    }

    public void SpawnOnNextLocation(UnitStats stats)
    {
        if (stats.NextLocation == LOCATION.HUNTZONE
            && !GameManager.huntZoneManager.HuntZones.ContainsKey(stats.HuntZoneNum))
        {
            stats.SetLocation(LOCATION.NONE, LOCATION.VILLAGE);
            return;
        }

        switch (stats.NextLocation)
        {
            case LOCATION.NONE:
                break;
            case LOCATION.VILLAGE:
                GameManager.villageManager.village.UnitSpawn(stats.InstanceID);
                break;
            case LOCATION.HUNTZONE:
                GameManager.huntZoneManager.HuntZones[stats.HuntZoneNum].SpawnUnit(stats.InstanceID);
                break;
        }
    }
    public void SpawnOnLocation(UnitStats stats)
    {
        if (stats.Location == LOCATION.HUNTZONE
            && !GameManager.huntZoneManager.HuntZones.ContainsKey(stats.HuntZoneNum))
        {
            stats.SetLocation(LOCATION.NONE, LOCATION.VILLAGE);
            return;
        }
        switch (stats.Location)
        {
            case LOCATION.NONE:
                break;
            case LOCATION.VILLAGE:
                GameManager.villageManager.village.UnitSpawn(stats.InstanceID);
                break;
            case LOCATION.HUNTZONE:
                GameManager.huntZoneManager.HuntZones[stats.HuntZoneNum].SpawnUnit(stats.InstanceID);
                break;
        }
    }

    public void GachaCharacter(int level)
    {
        var waitCharacter = new UnitStats();
        waitCharacter.InitStats(GachaUnitData(level));
        waitCharacter.ResetStats();
        Waitings.Add(waitCharacter.InstanceID, waitCharacter);

        SaveManager.SaveGame();
    }

    public UnitStats PickUpCharacter(int instanceID)
    {
        var pick = Waitings[instanceID];
        Waitings.Remove(instanceID);

        if (Units.ContainsKey(pick.InstanceID))
            return null;

        Units.Add(pick.InstanceID, pick);
        pick.SetUpgradeStats();
        pick.SetLocation(LOCATION.NONE, LOCATION.VILLAGE);

        SaveManager.SaveGame();

        return pick;
    }
}