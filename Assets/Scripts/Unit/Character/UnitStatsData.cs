using System;
using UnityEngine;

[Serializable]
public class UnitStatsData : ITableAvaialable<int>
{
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public int Id { get; set; }
    [field: SerializeField] public int GachaChance { get; set; }
    [field: SerializeField] public int GachaLevel { get; set; }
    [field: SerializeField] public UNIT_JOB Job { get; set; }
    [field: SerializeField] public ATTACK_TYPE BasicAttackType { get; set; }
    [field: SerializeField] public int MaxHP { get; set; }
    [field: SerializeField] public int MaxStamina { get; set; }
    [field: SerializeField] public int MaxMental { get; set; }
    [field: SerializeField] public float RecognizeRange { get; set; }
    [field: SerializeField] public float SizeRange { get; set; }
    [field: SerializeField] public float PresenseRange { get; set; }
    [field: SerializeField] public float BasicAttackRange { get; set; }
    [field: SerializeField] public float MoveSpeed { get; set; }
    [field: SerializeField] public int StrMin { get; set; }
    [field: SerializeField] public int StrMax { get; set; }
    [field: SerializeField] public float StrWeight { get; set; }
    [field: SerializeField] public int WizMin { get; set; }
    [field: SerializeField] public int WizMax { get; set; }
    [field: SerializeField] public float WizWeight { get; set; }
    [field: SerializeField] public int AgiMin { get; set; }
    [field: SerializeField] public int AgiMax { get; set; }
    [field: SerializeField] public float AgiWeight { get; set; }
    [field: SerializeField] public float AttackSpeed { get; set; }
    [field: SerializeField] public float CritChance { get; set; }
    [field: SerializeField] public float CritWeight { get; set; }
    [field: SerializeField] public int VitMin { get; set; }
    [field: SerializeField] public int VitMax { get; set; }
    [field: SerializeField] public float VitWeight { get; set; }
    [field: SerializeField] public int SkillPoolId1 { get; set; }
    [field: SerializeField] public int SkillPoolId2 { get; set; }
    [field: SerializeField] public string UnitAssetFileName { get; set; }

    public int TableID => Id;
}