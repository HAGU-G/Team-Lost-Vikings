using System.Collections.Generic;

public class SkillPool
{
    public string Name { get; set; }
    public int Id { get; set; }
    public int SkillPoolDivision { get; set; } //더미데이터

    /// <summary>
    /// 스킬ID, 가중치
    /// </summary>
    public Dictionary<int, int> Pool { get; set; }
}