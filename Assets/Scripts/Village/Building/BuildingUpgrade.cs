﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Building))]
public class BuildingUpgrade : MonoBehaviour
{
    [field: SerializeField] public string UpgradeName { get; private set; }
    [field: SerializeField] public int UpgradeId { get; private set; }
    [field: SerializeField] public int UpgradeGrade { get; private set; }
    [field: SerializeField] public int StructureLevel { get; private set; }
    [field: SerializeField] public int StructureType { get; private set; }
    [field: SerializeField] public STAT_TYPES StatType { get; private set; }
    [field: SerializeField] public int StatReturn { get; private set; }
    [field: SerializeField] public int ParameterType { get; private set; }
    [field: SerializeField] public int ParameterRecovery { get; private set; }
    [field: SerializeField] public float RecoveryTime { get; private set; }
    [field: SerializeField] public int ProgressVarType { get; private set; }
    [field: SerializeField] public float ProgressVarReturn { get; private set; }
    [field: SerializeField] public int RecipeId { get; private set; }
    [field: SerializeField] public int ItemStack { get; private set; }
    [field: SerializeField] public float RequireTime { get; private set; }
    [field: SerializeField] public int RequireGold { get; private set; }
    [field: SerializeField] public int RequireRune { get; private set; }
    [field: SerializeField] public List<int> ItemIds { get; private set; }
    [field: SerializeField] public List<int> ItemNums { get; private set; }
    [field: SerializeField] public string UpgradeDesc { get; private set; }

    public int currentGrade = 1;

    private void Awake()
    {
        GameManager.Subscribe(EVENT_TYPE.START,SetBuildingUpgrade);
    }

    public void SetBuildingUpgrade()
    {
        Debug.Log("ㅇㅇㄹㄴㅇㄹ");
        UpgradeId = GetComponent<Building>().UpgradeId;

        var upgrade = DataTableManager.upgradeTable.GetData(UpgradeId)[2];

        UpgradeGrade = upgrade.UpgradeGrade;
        UpgradeName = upgrade.UpgradeName;
        StatType = upgrade.StatType;
        StatReturn = upgrade.StatReturn;
        ParameterRecovery = upgrade.ParameterRecovery;
        RecoveryTime = upgrade.RecoveryTime;
        ProgressVarType = upgrade.ProgressVarType;
        ProgressVarReturn = upgrade.ProgressVarReturn;
        RecipeId = upgrade.RecipeId;
        ItemStack = upgrade.ItemStack;
        RequireTime = upgrade.RequireTime;
        RequireGold = upgrade.RequireGold;
        RequireRune = upgrade.RequireRune;

        ItemIds.Clear();
        ItemNums.Clear();
        for (int i = 0; i < 5; ++i)
        {
            ItemIds.Add(upgrade.ItemIds[i]);
            ItemNums.Add(upgrade.ItemNums[i]);
        }

        UpgradeDesc = upgrade.UpgradeDesc;

    }

    public void Upgrade()
    {
        switch (StructureType)
        {
            case (int)STRUCTURE_TYPE.STAT_UPGRADE:
                var stat = GetComponent<StatUpgradeBuilding>();
                if (StatType == stat.upgradeStat)
                {
                    stat.upgradeValue = StatReturn;
                    ++currentGrade;
                    stat.RiseStat();
                }
                break;
            case (int)STRUCTURE_TYPE.PARAMETER_RECOVERY:
                var parameter = GetComponent<ParameterRecoveryBuilding>();
                if ((PARAMETER_TYPES)ParameterType == parameter.parameterType)
                {
                    parameter.recoveryAmount += ParameterRecovery;
                    parameter.recoveryTime = RecoveryTime;
                    ++currentGrade;
                }
                break;
            case (int)STRUCTURE_TYPE.ITEM_PRODUCE:
                break;
            case (int)STRUCTURE_TYPE.ITEM_SELL:
                break;
        }
        SetBuildingUpgrade();
    }

}
