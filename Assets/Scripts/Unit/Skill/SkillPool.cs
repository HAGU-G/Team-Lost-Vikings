using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;

public class SkillPool : ITableAvaialable<int>
{
    public string Name { get; set; }
    public int Id { get; set; }
    public int SkillId { get; set; }
    public int SkillGachaChance { get; set; }

    public int TableID => Id;
}