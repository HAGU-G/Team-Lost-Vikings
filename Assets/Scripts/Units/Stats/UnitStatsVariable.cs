using System;
using UnityEngine;

[Serializable]
public class UnitStatsVariable
{
    //Parameters
    [field: SerializeField] public int MaxStamina { get; set; }
    [field: SerializeField] public int MaxStress { get; set; }

    //Stats
    [field: SerializeField] public int BaseHP { get; set; }
    [field: SerializeField] public int Vit { get; set; }
    [field: SerializeField] public float VitWeight { get; set; }


    [field: SerializeField] public int Str { get; set; }
    [field: SerializeField] public float StrWeight { get; set; }
    [field: SerializeField] public int Mag { get; set; }
    [field: SerializeField] public float MagWeight { get; set; }
    [field: SerializeField] public int Agi { get; set; }
    [field: SerializeField] public float AgiWeight { get; set; }

    [field: SerializeField] public float UnitSize { get; set; }
    [field: SerializeField] public float MoveSpeed { get; set; }
    [field: SerializeField] public float RecognizeRange { get; set; }

    [field: SerializeField] public float AttackRange { get; set; }
    [field: SerializeField] public float AttackSpeed { get; set; }

    [field: SerializeField] public float CriticalChance { get; set; }
    [field: SerializeField] public float CriticalWeight { get; set; }

    [field: SerializeField] public int CombatPoint { get; private set; }
    public void UpdateCombatPoint()
    {
        CombatPoint = StatMath.CalculateCombatPoint(this);
    }



    public UnitStatsVariable Clone()
    {
        var clone = new UnitStatsVariable();

        clone.MaxStress = MaxStress;
        clone.MaxStamina = MaxStamina;

        clone.BaseHP = BaseHP;
        clone.Vit = Vit;
        clone.VitWeight = VitWeight;

        clone.Str = Str;
        clone.StrWeight = StrWeight;
        clone.Mag = Mag;
        clone.MagWeight = MagWeight;
        clone.Agi = Agi;
        clone.AgiWeight = AgiWeight;

        clone.UnitSize = UnitSize;
        clone.MoveSpeed = MoveSpeed;
        clone.RecognizeRange = RecognizeRange;
        clone.AttackRange = AttackRange;
        clone.AttackSpeed = AttackSpeed;

        clone.CriticalChance = CriticalChance;
        clone.CriticalWeight = CriticalWeight;

        return clone;
    }

    public UnitStatsVariable AddStats(UnitStatsVariable stats)
    {
        MaxStress += stats.MaxStress;
        MaxStamina += stats.MaxStamina;

        BaseHP += stats.BaseHP;
        Vit+= stats.Vit;
        VitWeight += stats.VitWeight;

        Str += stats.Str;
        StrWeight += stats.StrWeight;
        Mag += stats.Mag;
        MagWeight += stats.MagWeight;
        Agi += stats.Agi;
        AgiWeight += stats.AgiWeight;

        UnitSize += stats.UnitSize;
        MoveSpeed += stats.MoveSpeed;
        RecognizeRange += stats.RecognizeRange;
        AttackRange += stats.AttackRange;
        AttackSpeed += stats.AttackSpeed;    

        CriticalChance += stats.CriticalChance;
        CriticalWeight += stats.CriticalWeight;

        return this;
    }
    public UnitStatsVariable ResetStats()
    {
        MaxStress = 0;
        MaxStamina = 0;

        BaseHP = 0;
        Vit = 0;
        VitWeight = 0f;

        Str = 0;
        StrWeight = 0f;
        Mag = 0;
        MagWeight = 0f;
        Agi = 0;
        AgiWeight = 0f;

        UnitSize = 0f;
        MoveSpeed = 0f;
        RecognizeRange = 0f;
        AttackRange = 0f;
        AttackSpeed = 0f;

        CriticalChance = 0f;
        CriticalWeight = 0f;

        return this;
    }
}