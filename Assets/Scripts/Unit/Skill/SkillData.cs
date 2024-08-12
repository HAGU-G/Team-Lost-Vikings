public enum SKILL_DIVISION
{
    PASSIVE,
    ACTIVE,
}

public enum SKILL_ACTIVE_TYPE
{
    NONE = -1,
    ALWAYS,
    COOLTIME,
    BASIC_ATTACK_PROBABILITY,
    BASIC_ATTACK_COUNT
}

public enum STAT_VALUE_TYPE
{
    NONE,
    ADD,
    RATIO,
}

public enum TARGET_TYPE
{
    OWN,
    TEAM,
    TEAM_ALL,
    ENEMY
}

public enum SKILL_ATTACK_TYPE
{
    NONE,
    SINGLE,
    RANGE,
    FLOOR,
    PROJECTILE
}



public class SkillData : ITableAvaialable<int>
{
    public int SkillId { get; set; }
    public ATTACK_TYPE SkillType { get; set; }
    public TARGET_TYPE SkillTarget { get; set; }
    public SKILL_ACTIVE_TYPE SkillActiveType { get; set; }
    public SKILL_ATTACK_TYPE SkillAttackType { get; set; }
    public float SkillCastTime { get; set; }
    public float SkillCastRange { get; set; }
    public float SkillAttackRange { get; set; }
    public int SkillActiveNum { get; set; }
    public float SkillFloorActiveTerm { get; set; }

    public float SkillActiveValue { get; set; }
    public float BuffRange { get; set; }
    public STAT_TYPE BuffType { get; set; }
    public STAT_VALUE_TYPE BuffStatValueType { get; set; }
    public float BuffStatValue { get; set; }
    public float SkillDuration { get; set; }
    public float ProjectileSpeed { get; set; }

    public float SkillDmgRatio { get; set; }
    public float SkillStrRatio { get; set; }
    public float SkillWizRatio { get; set; }
    public float SkillAgiRatio { get; set; }
    public float SkillFloorDmgRatio { get; set; }
    public float VitDrainRatio { get; set; }

    public string SkillName { get; set; }
    public string SkillDesc { get; set; }
    public string SkillDetail { get; set; }

    public string SkillEffectName { get; set; }
    public string SkillIconName { get; set; }
    public string SkillSEName { get; set; }
    public ATTACK_MOTION SkillAnime { get; set; }
    public string ProjectileFileType { get; set; }
    public string ProjectileFileName {  get; set; }

    //public float SkillRankPoint { get; set; }

    public int TableID => SkillId;
}
