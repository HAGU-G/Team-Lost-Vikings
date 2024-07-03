using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public abstract class Unit : MonoBehaviour
{
    protected UnitStats stats;

    protected virtual void Init()
    {
        stats.Init();
    }
}
