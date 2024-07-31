using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Building))]
public class BuildingUpgrade : MonoBehaviour
{
    public string UpgradeName { get; set; }
    public int UpgradeId { get; set; }
    public int UpgradeGrade { get; set; }
    public int StructureLevel { get; set; }
    public int StructureType { get; set; }
    public STAT_TYPE StatType { get; set; }
    public int StatReturn { get; set; }
    public int ParameterType { get; set; }
    public int ParameterRecovery { get; set; }
    public float RecoveryTime { get; set; }
    public int ProgressVarType { get; set; }
    public float ProgressVarReturn { get; set; }
    public int RecipeId { get; set; }
    public int ItemStack { get; set; }
    public float RequireTime { get; set; }
    public int RequireGold { get; set; }
    public int RequireRune { get; set; }
    public List<int> ItemIds { get; set; }
    public List<int> ItemNums { get; set; }
    public string UpgradeDesc { get; set; }

    public int currentGrade = 1;

    private void Start()
    {
        GameManager.Subscribe(EVENT_TYPE.START, SetBuildingUpgrade);
    }

    public void SetBuildingUpgrade()
    {
        UpgradeId = GetComponent<Building>().UpgradeId;
        if (UpgradeId == 0)
            return;

        UpgradeData upgrade = UpgradeData.GetUpgradeData(UpgradeId, currentGrade);
        if (upgrade == null)
        {
            Debug.Log($"업그레이드 {currentGrade}단계가 없습니다.", gameObject);
            return;
        }

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
                    ++currentGrade;
                    SetBuildingUpgrade();
                    stat.upgradeValue = StatReturn;

                    stat.RiseStat();
                }
                break;
            case (int)STRUCTURE_TYPE.PARAMETER_RECOVERY:
                var parameter = GetComponent<ParameterRecoveryBuilding>();
                if ((PARAMETER_TYPE)ParameterType == parameter.parameterType)
                {
                    ++currentGrade;
                    SetBuildingUpgrade();
                    parameter.recoveryAmount = ParameterRecovery;
                    parameter.recoveryTime = RecoveryTime;

                }
                break;
            case (int)STRUCTURE_TYPE.ITEM_PRODUCE:
                break;
            case (int)STRUCTURE_TYPE.ITEM_SELL:
                break;
            case (int)STRUCTURE_TYPE.STANDARD:
                ++currentGrade;
                SetBuildingUpgrade();
                break;
        }
    }

}
