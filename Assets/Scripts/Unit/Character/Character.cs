using System;
using UnityEngine;

[Serializable]
public abstract class Character : Unit
{
    public UnitStats stats = null;
    public override Stats GetStats => stats;

    public virtual void ResetUnit(UnitStats stats)
    {
        if (stats == null)
            Debug.LogWarning("유닛의 스탯이 재설정되지 않았습니다.", gameObject);
        else
            this.stats = stats;

        ResetBase();
    }

    public override void OnRelease()
    {
        base.OnRelease();
        stats = null;
    }
}
