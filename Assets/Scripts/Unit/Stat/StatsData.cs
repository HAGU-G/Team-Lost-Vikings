public enum STAT_TYPE
{
    NONE,
    STR,
    WIZ,
    AGI,
    CRIT_CHANCE,
    CRIT_WEIGHT,
    HP,
    STAMINA,
    MENTAL,
    WARRIOR_WEIGHT,
    MAGICIAN_WEIGHT,
    ARCHER_WEIGHT,
    ATTACK_SPEED,
    VIT,
}

public enum PARAMETER_TYPE
{
    NONE,
    HP,
    STAMINA,
    MENTAL,
}

public enum UNIT_JOB
{
    NONE,
    WARRIOR,
    MAGICIAN,
    ARCHER
}

public enum ATTACK_TYPE
{
    NONE,
    PHYSICAL,
    MAGIC,
    SPECIAL,
}

public enum ATTACK_MOTION
{
    NONE,
    BASIC,
    MAGIC,
    BOW
}

public enum UNIT_GRADE
{
    COMMON,
    NORMAL,
    RARE,
    SUPER_RARE,
    ULTRA_RARE
}

public enum UNIT_TYPE
{
    NONE,
    CHARACTER,
    MONSTER
}

public class StatsData : ITableAvaialable<int>
{
    public string Name { get; set; }
    public int Id { get; set; }
    public UNIT_TYPE UnitType { get; set; }

    public int GachaChance { get; set; }
    public int GachaUnlockLv { get; set; }
    public UNIT_JOB Job { get; set; }

    public int BaseHP { get; set; }
    public int BaseStamina { get; set; }
    public int BaseMental { get; set; }

    public float SizeRange { get; set; }
    public float PresenseRange { get; set; }
    public float RecognizeRange { get; set; }
    public float BasicAttackRange { get; set; }

    public float MoveSpeed { get; set; }
    public float AttackSpeed { get; set; }

    public ATTACK_TYPE BasicAttackType { get; set; }
    public ATTACK_MOTION BasicAttackMotion { get; set; }

    public int MinBaseStr { get; set; }
    public int MaxBaseStr { get; set; }
    public float StrWeight { get; set; }
    public int MinBaseWiz { get; set; }
    public int MaxBaseWiz { get; set; }
    public float WizWeight { get; set; }
    public int MinBaseAgi { get; set; }
    public int MaxBaseAgi { get; set; }
    public float AgiWeight { get; set; }
    public int MinBaseVit { get; set; }
    public int MaxBaseVit { get; set; }
    public float VitWeight { get; set; }

    public float CritChance { get; set; }
    public float CritWeight { get; set; }

    public int PhysicalDef { get; set; }
    public int MagicalDef { get; set; }
    public int SpecialDef { get; set; }

    public int SkillpoolId1 { get; set; }
    public int SkillpoolId2 { get; set; }

    public int DropId { get; set; }
    public string UnitAssetFileName { get; set; }
    public string StringId_Desc { get; set; }

    public int TableID => Id;
}