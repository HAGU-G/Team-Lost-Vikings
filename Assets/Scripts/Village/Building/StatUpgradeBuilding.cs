using System.Collections.Generic;
using UnityEngine;


public class StatUpgradeBuilding : MonoBehaviour, IInteractableWithPlayer
{
    public Building building;
    public STAT_TYPE upgradeStat;
    public int upgradeValue;
    private List<UnitOnVillage> units;
    private StatInt statInt = new();

    private void Awake()
    {
        building = GetComponent<Building>();
        GameManager.Subscribe(EVENT_TYPE.START, OnGameStart);
    }

    private void OnGameStart()
    {
        RiseStat();
    }

    public void InteractWithPlayer()
    {
        //UI 띄우기
    }

    public void RiseStat()
    {
        switch (upgradeStat)
        {
            case STAT_TYPE.STR:
                //foreach(var unit in units)
                //{
                //    statInt.defaultValue = upgradeValue;
                //    unit.stats.Str.upgradeValue = statInt;
                //}
                GameManager.playerManager.unitStr.defaultValue = upgradeValue;
                break;
            case STAT_TYPE.WIZ:
                //foreach (var unit in units)
                //{
                //    statInt.defaultValue = upgradeValue;
                //    unit.stats.Mag.upgradeValue = statInt;
                //}
                GameManager.playerManager.unitMag.defaultValue = upgradeValue;
                break;
            case STAT_TYPE.AGI:
                //foreach (var unit in units)
                //{
                //    statInt.defaultValue = upgradeValue;
                //    unit.stats.Agi.upgradeValue = statInt;
                //}
                GameManager.playerManager.unitAgi.defaultValue = upgradeValue;
                break;
        }
    }

    public void SetUnits(List<UnitOnVillage> units)
    {
        this.units = units;
    }
}
