using System.Collections.Generic;
using UnityEngine;

public static class DataTableManager
{
    public static Dictionary<int, UnitStatsData> characterTable = new();
    public static Dictionary<int, SkillData> skillTable = new();



    //TESTCODE
    static DataTableManager()
    {
        for (int i = 1; i <= 3; i++)
        {
            characterTable.Add(i, new UnitStatsData()
            {
                Name = $"용병{i}",
                Id = i,
                GachaChance = i,
                GachaLevel = 0,
                Job = (UNIT_JOB)(i),
                BasicAttackType = (ATTACK_TYPE)(i - 1),
                MaxHP = 50 * i,
                MaxStamina = 10 * i,
                MaxMental = 10 * i,
                RecognizeRange = 7 + i,
                BasicAttackRange = 0.5f * i,
                SizeRange = 0.4f,
                MoveSpeed = 3f + i,
                StrMin = 5 * i,
                StrMax = 10 * i,
                StrWeight = 1.0f + 0.1f * i,
                WizMin = 1 * i,
                WizMax = 10 * i,
                WizWeight = 0.5f + 0.1f * i,
                AgiMin = 20 * i,
                AgiMax = 30 * i,
                AgiWeight = 0.2f + 0.05f * i,
                AttackSpeed = 0.2f * i,
                CritChance = 0.3f * i,
                CritWeight = 2.0f - 0.3f * i,
                VitMin = 40 + i,
                VitMax = 200 + i,
                VitWeight = 1.0f + 0.1f * i,
                SkillPoolId1 = 0,
                SkillPoolId2 = 0,
                UnitAssetFileName = (UNIT_JOB)(i) switch
                {
                    UNIT_JOB.WARRIOR => "10_RoyalKnight",
                    UNIT_JOB.MAGICIAN => "2_Magician",
                    UNIT_JOB.ARCHER => "6_Hunter",
                    _ => ""
                }
            });

        }
    }
}
