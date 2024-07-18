using System;
using UnityEngine;

[Serializable]
public abstract class Unit : MonoBehaviour, IStatUsable
{
    public UnitStats stats;
    public virtual STAT_GROUP StatGroup => STAT_GROUP.UNIT_ON_VILLAGE;
    public Stats GetStats => stats;
    public UnitSkills skills;
    public GameObject skillEffect;

    /// <summary>
    /// 오브젝트 풀 OnCreate에서 호출
    /// </summary>
    public virtual void Init()
    {
    }

    /// <summary>
    /// 오브젝트 풀 OnGet에서 호출
    /// </summary>
    public virtual void ResetUnit(UnitStats unitStats)
    {
        if (unitStats == null)
            Debug.LogWarning("유닛의 스탯이 재설정되지 않았습니다.", gameObject);
        else
            stats = unitStats;

        stats.ResetEllipse(transform);
        ResetEvents();
    }

    /// <summary>
    /// 오브젝트 풀 OnRelease에서 호출
    /// </summary>
    public virtual void OnRelease()
    {
        stats.SetLocation(LOCATION.NONE);
        stats = null;
    }

    public virtual void RemoveUnit()    {    }

    protected virtual void ResetEvents() { }
}
