using System;
using System.Collections;
using System.Collections.Generic;

public class UnitManager
{
    public Dictionary<int, UnitStats> Units { get; private set; } = new();

    public void LoadUnits()
    {
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
            return null;

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
}