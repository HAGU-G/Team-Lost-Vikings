using System;
using UnityEngine;

public enum SKILL_DIVISION
{
    PASSIVE,
    ACTIVE,
}
public enum SKILL_ACTIVE_TYPE
{
    NONE,
    ALWAYS,
    COOLTIME,
    BASIC_ATTACK_PROBABILITY,
    BASIC_ATTACK_COUNT
}

public enum STAT_TYPE
{
    NONE,
    STR,
    WIZ,
    AGI,
    VIT
}

public enum PARAMETER_TYPE
{
    NONE,
    BASE_HP,
    STAMINA,
    STRESS,
}

public enum RETURN_TYPE
{
    NONE,
    ADD,
    RATIO,
}

[Serializable]
public class SkillData : ITableAvaialable<int>
{
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public int Id { get; set; }
    [field: SerializeField] public float SkillRankPoint { get; set; }
    [field: SerializeField] public SKILL_DIVISION SkillDivision { get; set; }
    [field: SerializeField] public SKILL_ACTIVE_TYPE ActiveType { get; set; }
    [field: SerializeField] public float ActiveValue { get; set; }
    [field: SerializeField] public float CastRange { get; set; }
    [field: SerializeField] public float CastTime { get; set; }
    [field: SerializeField] public ATTACK_TYPE SkillAttackType { get; set; }
    [field: SerializeField] public float SkillDmgRatio { get; set; }
    [field: SerializeField] public int SkillAttackNum { get; set; }
    [field: SerializeField] public STAT_TYPE StatType { get; set; }
    [field: SerializeField] public PARAMETER_TYPE ParameterType { get; set; }
    [field: SerializeField] public RETURN_TYPE PassiveReturnType { get; set; }
    [field: SerializeField] public float PassiveValue { get; set; }
    [field: SerializeField] public string SkillIconAssetFileName { get; set; }
    [field: SerializeField] public string SkillEffectAssetFileName { get; set; }

    public int TableID => Id;
}
