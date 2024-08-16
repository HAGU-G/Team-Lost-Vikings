public enum ACHIEVEMENT_TYPE
{
    NONE,
    BUILDING_BUILD,
    BUILDING_UPGRADE,
    MONSTER_KILL,
    ITEM_GET,
    ITEM_USE
}

public class AchievementData : ITableAvaialable<int>
{
    public string AchieveName { get; set; }
    public int AchieveId { get; set; }
    public string AchieveDef { get; set; }
    public ACHIEVEMENT_TYPE AchieveType { get; set; }
    public int TargetId { get; set; }
    public int TableID => AchieveId;
}