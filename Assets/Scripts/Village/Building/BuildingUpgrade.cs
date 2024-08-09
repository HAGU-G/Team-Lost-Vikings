using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Building))]
public class BuildingUpgrade : MonoBehaviour
{
    [field: SerializeField]
    public string UpgradeName { get; set; }
    [field: SerializeField]
    public int UpgradeId { get; set; }
    [field: SerializeField]
    public int UpgradeGrade { get; set; }
    [field: SerializeField]
    public int StructureLevel { get; set; }
    [field: SerializeField]
    public int StructureType { get; set; }
    [field: SerializeField]
    public STAT_TYPE StatType { get; set; }
    [field: SerializeField]
    public int StatReturn { get; set; }
    [field: SerializeField]
    public int ParameterType { get; set; }
    [field: SerializeField]
    public int ParameterRecovery { get; set; }
    [field: SerializeField]
    public float RecoveryTime { get; set; }
    [field: SerializeField]
    public int ProgressVarType { get; set; }
    [field: SerializeField]
    public float ProgressVarReturn { get; set; }
    [field: SerializeField]
    public int RecipeId { get; set; }
    [field: SerializeField]
    public int ItemStack { get; set; }
    [field: SerializeField]
    public float RequireTime { get; set; }
    [field: SerializeField]
    public int RequireGold { get; set; }
    [field: SerializeField]
    public int RequireRune { get; set; }
    [field: SerializeField]
    public List<int> ItemIds { get; set; }
    [field: SerializeField]
    public List<int> ItemNums { get; set; }
    [field: SerializeField]
    public string UpgradeDesc { get; set; }

    public int currentGrade = 1;

    private void Start()
    {
        //GameManager.Subscribe(EVENT_TYPE.START, SetBuildingUpgrade);
    }

    public void SetBuildingUpgrade()
    {
        var building = GetComponent<Building>();
        UpgradeId = building.UpgradeId;
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

        if (GameManager.playerManager.buildingUpgradeGrades.TryGetValue(building.StructureId, out int value))
        {
            value = currentGrade;
        }
        else
        {
            GameManager.playerManager.buildingUpgradeGrades.Add(building.StructureId, currentGrade);
        }
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
            case (int)STRUCTURE_TYPE.STANDARD:
                GameManager.villageManager.LevelUp();
                ++currentGrade;
                SetBuildingUpgrade();
                break;
            //case (int)STRUCTURE_TYPE.PARAMETER_UPGRADE:
            //    var paramUp = GetComponent<ParameterUpgradeBuilding>();
            //    if(StatType == paramUp.upgradeParameter)
            //    {
            //        ++currentGrade;
            //        SetBuildingUpgrade();
            //        paramUp.upgradeValue = StatReturn;
            //        paramUp.RiseParameter();
            //    }
            //    break;
            case (int)STRUCTURE_TYPE.REVIVE:
                var reviveTime = ProgressVarReturn;
                var revive = GetComponent<ReviveBuilding>();
                revive.reviveTime = reviveTime;
                break;
        }
    }
}
