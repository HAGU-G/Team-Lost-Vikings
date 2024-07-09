using System;
using UnityEngine;

[Serializable]
public abstract class Unit : MonoBehaviour
{
    public UnitStatsData testData;
    public UnitStats stats;

    /// <summary>
    /// base.Init()가 최상단에 있어야함.
    /// </summary>
    protected virtual void Init() 
    {
        stats = new(testData);
        
    }

    /// <summary>
    /// base.ResetUnit()이 최상단에 있어야함.
    /// </summary>
    protected virtual void ResetUnit()
    {
        stats.ResetStats();
    }
}
