using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;

public class SkillPool : ITableAvaialable<int>
{
    public string Name { get; set; }
    public int Id { get; set; }
    public int SkillPoolDivision { get; set; }
    public int SkillId { get; set; }
    public int SkillGachaChance { get; set; }

    /// <summary>
    /// 스킬ID, 가중치
    /// </summary>
    [Ignore] public Dictionary<int, int> Pool { get; set; }

    public int TableID => Id;
}