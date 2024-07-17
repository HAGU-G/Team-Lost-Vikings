using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StatUpgradeBuilding : MonoBehaviour, IInteractableWithPlayer
{
    public Building building;
    public STAT_TYPES upgradeStat; //나중에 데이터형 수정하기
    public int upgradeValue;
    private List<UnitOnVillage> units;
    private StatInt statInt = new();

    private void Awake()
    {
        building = GetComponent<Building>();
    }

    public void InteractWithPlayer()
    {
        //UI 띄우기
    }

    public void RiseStat()
    {
        switch (upgradeStat)
        {
            case STAT_TYPES.STR:
                foreach(var unit in units)
                {
                    statInt.defaultValue = upgradeValue;
                    unit.stats.Str.upgradeValue = statInt;
                }
                break;
            case STAT_TYPES.MAG:
                foreach (var unit in units)
                {
                    statInt.defaultValue = upgradeValue;
                    unit.stats.Mag.upgradeValue = statInt;
                }
                break;
            case STAT_TYPES.AGI:
                foreach (var unit in units)
                {
                    statInt.defaultValue = upgradeValue;
                    unit.stats.Agi.upgradeValue = statInt;
                }
                break;
        }
    }

    public void SetUnits(List<UnitOnVillage> units)
    {
        this.units = units;
    }
}
