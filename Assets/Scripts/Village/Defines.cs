using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum STRUCTURE_TYPE
{
    HOSPITAL = -1,
    STAT_UPGRADE,
    PARAMETER_RECOVERY,
    ITEM_PRODUCE,
    ITEM_SELL,
    REVIVE,
}

public enum RECIPE
{
    RECIPE1, //임시로 넣은 것임
    RECIPE2,
    RECIPE3,
}

public enum PARAMETER_TYPES
{
    NONE = 0,
    HP,
    STAMINA,
    STRESS,
}

public enum STATTYPES
{
    STR,
    MAG,
    AGI,
}