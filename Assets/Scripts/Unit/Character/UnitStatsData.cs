using System;
using UnityEngine;

[Serializable]
public class UnitStatsData
{
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public int Id { get; set; }
    [field: SerializeField] public float GachaChance { get; set; }
    [field: SerializeField] public int GachaLevel { get; set; }
    [field: SerializeField] public UNIT_JOB Job { get; set; }
    [field: SerializeField] public ATTACK_TYPE BasicAttackType { get; set; }
    [field: SerializeField] public int HP { get; set; }
    [field: SerializeField] public int Stamina { get; set; }
    [field: SerializeField] public int Stress { get; set; }
    [field: SerializeField] public float RecognizeRange { get; set; }
    [field: SerializeField] public float BasicAttackRange { get; set; }
    [field: SerializeField] public float SizeRange { get; set; }
    [field: SerializeField] public float MoveSpeed { get; set; }
    [field: SerializeField] public int StrMin { get; set; }
    [field: SerializeField] public int StrMax { get; set; }
    [field: SerializeField] public float StrWeight { get; set; }
    [field: SerializeField] public int MagMin { get; set; }
    [field: SerializeField] public int MagMax { get; set; }
    [field: SerializeField] public float MagWeight { get; set; }
    [field: SerializeField] public int AgiMin { get; set; }
    [field: SerializeField] public int AgiMax { get; set; }
    [field: SerializeField] public float AgiWeight { get; set; }
    [field: SerializeField] public float AttackSpeed { get; set; }
    [field: SerializeField] public float CriticalChance { get; set; }
    [field: SerializeField] public float CriticalHitWeight { get; set; }
    [field: SerializeField] public int VitMin { get; set; }
    [field: SerializeField] public int VitMax { get; set; }
    [field: SerializeField] public float VitWeight { get; set; }
    [field: SerializeField] public int SkillId1 { get; set; }
    [field: SerializeField] public int SkillId2 { get; set; }
    [field: SerializeField] public string UnitAssetFileName { get; set; }
}