using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public abstract class Unit : MonoBehaviour
{
    public UnitStats stats;

    protected virtual void ResetUnit()
    {
        stats.ResetStats();
    }
}
