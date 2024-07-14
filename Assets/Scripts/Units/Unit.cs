using System;
using UnityEngine;

[Serializable]
public abstract class Unit : MonoBehaviour
{
    public UnitStats stats;
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
        stats = new(testData);
    }

    /// <summary>
    /// base.ResetUnit()이 최상단에 있어야함.
    /// </summary>
    protected virtual void ResetUnit()
    {
        ResetEvents();
        stats.ResetUnitStats();
        skills.ResetSkills();
    }

    /// <summary>
    /// base.ResetEvents()가 최상단에 있어야함.
    /// </summary>
    protected virtual void ResetEvents() { }
}
