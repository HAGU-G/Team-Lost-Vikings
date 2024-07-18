using System;
using System.Collections.Generic;

public class UnitManager
{
    public Dictionary<int, UnitStats> Units { get; private set; } = new();

    //TESTCODE 데이터테이블에서 가져오도록 변경
    public UnitStatsData unitStatsData;
    public void LoadUnits()
    {
        for (int i = 0; i < 10; i++)
        {
            var uu = new UnitStats();
            uu.InitStats(unitStatsData);
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
}