using System;
using UnityEngine;

[Serializable]
public abstract class Unit : MonoBehaviour
{
    public UnitStats stats;

    protected virtual void Init()    {    }

    /// <summary>
    /// base.ResetUnit()이 최상단에 있어야함.
    /// </summary>
    protected virtual void ResetUnit()
    {
        stats.ResetStats();
    }
}
