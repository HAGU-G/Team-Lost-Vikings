public enum STRUCTURE_TYPE
{
    STAT_UPGRADE,
    PARAMETER_RECOVERY,
    ITEM_PRODUCE,
    ITEM_SELL,
    STANDARD,
    PORTAL,
    PARAMETER_UPGRADE,
    REVIVE,
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
    HP_RECOVERY = 990003,
    STAMINA_RECOVERY = 990004,
    STRESS_RECOVERY = 990005,
    STANDARD = 990001,
    STR_UPGRADE = 990006,
    MAG_UPGRADE = 990007,
    AGI_UPGRADE = 990008,
    PORTAL = 990002,
    REVIVE = 990009,
}