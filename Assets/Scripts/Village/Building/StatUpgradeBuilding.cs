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
                GameManager.playerManager.unitCritWeight.defaultValue = upgradeValue * 0.01f;
                break;
            case STAT_TYPE.HP:
                GameManager.playerManager.unitHp.defaultValue = upgradeValue;
                break;
            case STAT_TYPE.STAMINA:
                GameManager.playerManager.unitStamina.defaultValue = upgradeValue;
                break;
            case STAT_TYPE.MENTAL:
                GameManager.playerManager.unitMental.defaultValue = upgradeValue;
                break;
            case STAT_TYPE.WARRIOR_WEIGHT:
                GameManager.playerManager.warriorWeight.defaultValue = upgradeValue;
                break;
            case STAT_TYPE.MAGICIAN_WEIGHT:
                GameManager.playerManager.magicianWeight.defaultValue = upgradeValue;
                break;
            case STAT_TYPE.ARCHER_WEIGHT:
                GameManager.playerManager.archerWeight.defaultValue = upgradeValue;
                break;
        }
        GameManager.Publish(EVENT_TYPE.UPGRADE);
    }

    public void SetUpgradeStat(Building obj)
    {
        var buildingUpgrade = obj.GetComponent<BuildingUpgrade>();
        var id = obj.UpgradeId;
        var upgradeData = DataTableManager.upgradeTable.GetData(id);
        upgradeValue = upgradeData[buildingUpgrade.UpgradeGrade - 1].StatReturn;
        upgradeStat = upgradeData[buildingUpgrade.UpgradeGrade - 1].StatType;
    }

    public void SetUnits(List<UnitOnVillage> units)
    {
        this.units = units;
    }
}
