using System.Collections.Generic;
using UnityEngine;


public class StatUpgradeBuilding : MonoBehaviour, IInteractableWithPlayer
{
    public Building building;
    public STAT_TYPE upgradeStat;
    public int upgradeValue;
    private List<UnitOnVillage> units;
    private StatInt statInt = new();

    private void Start()
    {
        building = GetComponent<Building>();
        GameManager.Subscribe(EVENT_TYPE.START, OnGameStart);
    }

    private void OnGameStart()
    {
        var buildingUp = GetComponent<BuildingUpgrade>();
        buildingUp.SetBuildingUpgrade();
        upgradeValue = buildingUp.StatReturn;
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
                GameManager.playerManager.unitStr.defaultValue = upgradeValue;
                break;
            case STAT_TYPE.WIZ:
                GameManager.playerManager.unitMag.defaultValue = upgradeValue;
                break;
            case STAT_TYPE.AGI:
                GameManager.playerManager.unitAgi.defaultValue = upgradeValue;
                break;
            case STAT_TYPE.CRIT_CHANCE:
                GameManager.playerManager.unitCritChance.defaultValue = upgradeValue;
                break;
            case STAT_TYPE.CRIT_WEIGHT:
                GameManager.playerManager.unitCritWeight.defaultValue = upgradeValue;
                break;
        }
        GameManager.Publish(EVENT_TYPE.UPGRADE);
    }

    public void SetUnits(List<UnitOnVillage> units)
    {
        this.units = units;
    }
}
