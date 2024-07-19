public enum STRUCTURE_TYPE
{
    STANDARD = -1,
    STAT_UPGRADE,
    PARAMETER_RECOVERY,
    ITEM_PRODUCE,
    ITEM_SELL,
    REVIVE,
}

public enum RECIPE
{
    RECIPE1, //임시로 넣은 것임
    RECIPE2,
    RECIPE3,
}

public enum PARAMETER_TYPES
{
    NONE = -1,
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
    HP_RECOVERY,
    STAMINA_RECOVERY,
    STRESS_RECOVERY,
    STANDARD,
    STR_UPGRADE,
    MAG_UPGRADE,
    AGI_UPGRADE,
    PORTAL,
}