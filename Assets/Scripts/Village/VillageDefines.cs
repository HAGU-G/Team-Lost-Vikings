public enum STRUCTURE_TYPE
{
    NONE = 0,
    STAT_UPGRADE,
    PARAMETER_RECOVERY,
    PROGRESS,
    REVIVE,
    PORTAL,
    STANDARD,
}

public enum PROGRESS_TYPE
{
    NONE = 0,
    HOTEL,
    REVIVE,
    STORAGE,
    RECRUIT,
}

public enum RECIPE
{
    RECIPE1, //임시로 넣은 것임
    RECIPE2,
    RECIPE3,
}

// NONE이 없어서 유닛 스탯에서 사용하던 STAT_TYPE을 사용하도록 변경
// 이에 맞춰 PARAMETER_TYPES도 유닛 스탯에서 쓰는 PARAMETER_TYPE을 사용하도록 변경
//public enum STAT_TYPES
//{
//    STR,
//    MAG,
//    AGI,
//}

public static class BuildingName
{
    public readonly static string hpRecovery = "hpRecovery";
    public readonly static string staminaRecovery = "staminaRecovery";
    public readonly static string stressRecovery = "stressRecovery";
}

public enum STRUCTURE_ID
{
    STANDARD = 1060001,
    PORTAL = 1050001,
    HP_RECOVERY = 1020001,
    STAMINA_RECOVERY = 1020002,
    STRESS_RECOVERY = 1020003,
    STR_UPGRADE = 1010001,
    MAG_UPGRADE = 1010002,
    AGI_UPGRADE = 1010003,
    REVIVE = 1040001,
    HP_UPGRADE = 1010004,
    STAMINA_UPGRADE = 1010005,
    MENTAL_UPGRADE = 1010006,
    RECRUIT = 1000001,
    HOTEL = 1000002,
    STORAGE = 1000003,
    WARRIOR_UPGRADE = 1010007,
    MAGICIAN_UPGRADE = 1010008,
    ARCHER_UPGRADE = 1010009,
    CRIT_CHANGE_UPGRADE = 1010010,
    CRIT_WEIGHT_UPGRADE = 1010011,
}