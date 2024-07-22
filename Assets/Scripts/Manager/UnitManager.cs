using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UnitManager
{
    /// <summary>
    /// Key: instanceID,
    /// Value: UnitStats,
    /// 꼭 필요한 경우가 아니라면 GetUnit() 메서드를 사용해주세요.
    /// </summary>
    public Dictionary<int, UnitStats> Units { get; private set; } = new();
    public Dictionary<int, UnitStats> DeadUnits { get; private set; } = new();
    public Dictionary<int, UnitStats> Waitings { get; private set; } = new();


    public void LoadUnits()
    {
        //TODO 세이브데이터에서 불러오도록 변경
        for (int i = 0; i < 20; i++)
        {
            GachaCharacter(GameManager.playerManager.level);
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

    public UnitStatsData GachaUnitData(int level)
    {
        var gachaList = new List<UnitStatsData>();

        foreach (var data in DataTableManager.characterTable)
        {
            if (level < data.Value.GachaLevel)
                continue;

            for (int i = 0; i < data.Value.GachaChance; i++)
            {
                gachaList.Add(data.Value);
            }
        }

        return gachaList[Random.Range(0, gachaList.Count)];
    }

    public void SpawnOnLocation(UnitStats stats)
    {
        switch (stats.NextLocation)
        {
            case LOCATION.NONE:
                break;
            case LOCATION.VILLAGE:
                GameManager.villageManager.village.UnitSpawn(stats.InstanceID);
                break;
            case LOCATION.HUNTZONE:
                GameManager.huntZoneManager.HuntZones[stats.HuntZoneID].SpawnUnit(stats.InstanceID);
                break;
        }
    }

    public void GachaCharacter(int level)
    {
        var waitCharacter = new UnitStats();
        waitCharacter.InitStats(GachaUnitData(level));
        waitCharacter.ResetStats();
        Waitings.Add(waitCharacter.InstanceID, waitCharacter);
    }

    public UnitStats PickUpCharacter(int instanceID)
    {
        var pick = Waitings[instanceID];
        Waitings.Remove(instanceID);
        pick.SetUpgradeStats();

        if (!Units.ContainsKey(pick.InstanceID))
            Units.Add(pick.InstanceID, pick);

        return pick;
    }
}