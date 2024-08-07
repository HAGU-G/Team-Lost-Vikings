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

public enum SKILL_TYPE
{
    NONE,
    SINGLE,
    RANGE,
    FLOOR,
    PROJECTILE
}



public class SkillData : ITableAvaialable<int>
{
    public int Id { get; set; }
    public ATTACK_TYPE SkillAttackType { get; set; }
    public TARGET_TYPE SkillTarget { get; set; }
    public SKILL_ACTIVE_TYPE ActiveType { get; set; }
    public SKILL_TYPE SkillType { get; set; }
    public float CastTime { get; set; }
    public float CastRange { get; set; }
    public float SkillAttackRange { get; set; }
    public int ActiveNum { get; set; }
    public float ActiveTerm { get; set; }

    public float ActiveValue { get; set; }
    public float BuffRange { get; set; }
    public STAT_TYPE BuffStateType { get; set; }
    public STAT_VALUE_TYPE BuffStateValueType { get; set; }
    public float BuffStateValue { get; set; }
    public float SkillDuration { get; set; }
    public float ProjectileSpeed { get; set; }

    public float SkillDmgRatio { get; set; }
    public float SkillStrRatio { get; set; }
    public float SkillWizRatio { get; set; }
    public float SkillAgiRatio { get; set; }
    public float FloorDmgRatio { get; set; }
    public float VitDrainRatio { get; set; }

    public string SkillName { get; set; }
    public string SkillDesc { get; set; }
    public string SkillDetail { get; set; }

    public string SkillEffectName { get; set; }
    public string SkillIconName { get; set; }
    public string SkillSEName { get; set; }
    public int SkillAnime { get; set; }

    //public float SkillRankPoint { get; set; }

    public int TableID => Id;
}
