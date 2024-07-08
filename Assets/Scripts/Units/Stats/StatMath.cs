using UnityEngine;

public static class StatMath
{
    public static int GetWeightedStat(int value, float weight, System.Func<float, int> correctionFunc = null)
    {
        correctionFunc ??= Mathf.FloorToInt;

        return correctionFunc(value * weight);
    }

    public static int CalculateCombatPoint(UnitStatsVariable stats)
    {
        return GetWeightedStat(stats.Str, stats.StrWeight)
            + GetWeightedStat(stats.Mag, stats.MagWeight)
            + GetWeightedStat(stats.Agi, stats.AgiWeight);
    }

    public static UNIT_GRADE CalulateGrade()
    {
        //TODO 유닛 등급 계산 필요

        return default;
    }

}
