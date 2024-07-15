using System;
using UnityEngine;

[Serializable]
public abstract class Unit : MonoBehaviour, IStatUsable
{
    public UnitStats stats = new();
    public virtual STAT_GROUP StatGroup => STAT_GROUP.UNIT_ON_VILLAGE;
    public Stats GetStats => stats;
    public UnitSkills skills;

    //TESTCODE
    public UnitStatsData testData;
    public SkillData testSkillData;
    public GameObject skillEffect;

    /// <summary>
    /// base.Init()가 최상단에 있어야함.
    /// </summary>
    protected virtual void Init()
    {
        //TESTCODE
        //TODO 스탯 할당, 스킬 할당
        stats.InitStats(testData);
        stats.InitEllipses(transform);
    }

    /// <summary>
    /// base.ResetUnit()이 최상단에 있어야함.
    /// </summary>
    protected virtual void ResetUnit()
    {
        ResetEvents();
        stats.ResetStats();
        stats.ResetEllipses();
        skills.ResetSkills();
    }

    /// <summary>
    /// base.ResetEvents()가 최상단에 있어야함.
    /// </summary>
    protected virtual void ResetEvents() { }
}
