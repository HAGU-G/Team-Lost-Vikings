using System;
using UnityEngine;

[Serializable]
public class MonsterStatsData
{
    [field: SerializeField] public string MonsterName { get; set; }
    [field: SerializeField] public int MonsterId { get; set; }
    [field: SerializeField] public float MoveSpeed { get; set; }
    [field: SerializeField] public float SizeRange { get; set; }
    [field: SerializeField] public float RecognizeRange { get; set; }
    [field: SerializeField] public float PresenseRange { get; set; }
    [field: SerializeField] public float BasicAttackRange { get; set; }
    [field: SerializeField] public int CombatPoint { get; set; }
    [field: SerializeField] public float AttackSpeed { get; set; }
    [field: SerializeField] public int MaxHP { get; set; }
    [field: SerializeField] public int PhysicalDef { get; set; }
    [field: SerializeField] public int MagicalDef { get; set; }
    [field: SerializeField] public int SpecialDef { get; set; }
    [field: SerializeField] public int DropId { get; set; }
    [field: SerializeField] public string MonsterAssetFileName { get; set; }
}