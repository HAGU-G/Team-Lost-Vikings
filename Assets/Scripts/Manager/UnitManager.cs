using System.Collections.Generic;
using UnityEngine;

public class UnitManager
{
    /// <summary>
    /// Key: instanceID,
    /// Value: UnitStats,
    /// 꼭 필요한 경우가 아니라면 GetUnit() 메서드를 사용해주세요.
    /// </summary>
    public Dictionary<int, UnitStats> Units { get; private set; } = new();
    private List<UnitStats> stanbyUnits = new();

    public void LoadUnits()
    {
        //TODO 세이브데이터에서 불러오도록 변경
        for (int i = 0; i < 10; i++)
        {
            var uu = new UnitStats();
            uu.InitStats(GachaCharacter(1));
            uu.ResetStats();
            Units.Add(uu.InstanceID, uu);
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

    public UnitStatsData GachaCharacter(int level)
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

        return gachaList[UnityEngine.Random.Range(0, gachaList.Count)];
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
}