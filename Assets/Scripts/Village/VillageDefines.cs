public enum STRUCTURE_TYPE
{
    STANDARD = -1,
    STAT_UPGRADE,
    PARAMETER_RECOVERY,
    ITEM_PRODUCE,
    ITEM_SELL,
    REVIVE,
    PORTAL,
}

public enum RECIPE
{
    RECIPE1, //임시로 넣은 것임
    RECIPE2,
    RECIPE3,
}

public enum PARAMETER_TYPES
{
    NONE = 0,
    HP,
    STAMINA,
    STRESS,
}

public enum STAT_TYPES
{
    STR,
    MAG,
    AGI,
}

public static class BuildingName
{
    public readonly static string hpRecovery = "hpRecovery";
    public readonly static string staminaRecovery = "staminaRecovery";
    public readonly static string stressRecovery = "stressRecovery";
}

public enum STRUCTURE_ID
{
    HP_RECOVERY = 110000001,
    STAMINA_RECOVERY = 110000002,
    STRESS_RECOVERY = 110000003,
    STANDARD = 3,
    STR_UPGRADE = 100000001,
    MAG_UPGRADE = 100000002,
    AGI_UPGRADE = 100000003,
    PORTAL = 7,
}