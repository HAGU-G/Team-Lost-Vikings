using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BuildingUpgrade : MonoBehaviour
{
    [field: SerializeField] public string UpgradeName {  get; private set; }
    [field: SerializeField] public int UpgradeId { get; private set; }
    [field: SerializeField] public int UpgradeGrade {  get; private set; }
    [field: SerializeField] public int StructureLevel { get; private set; }
    [field: SerializeField] public int StructureType { get; private set; }
    [field: SerializeField] public int StatType { get; private set; }
    [field: SerializeField] public int StatReturn { get; private set; }
    [field: SerializeField] public int ParameterType { get; private set; }
    [field: SerializeField] public int ParameterRecovery { get; private set; }
    [field: SerializeField] public float RecvoeryTime { get; private set; }
    [field: SerializeField] public int RecipeId { get; private set; }
    [field: SerializeField] public int ItemStack {  get; private set; }
    [field: SerializeField] public float RequireTime { get; private set; }
    [field: SerializeField] public int RequireGold { get; private set; }
    [field: SerializeField] public int RequireRune {  get; private set; }
    [field: SerializeField] public int ItemNum { get; private set; }
    [field: SerializeField] public int ItemId { get; private set; }

    public int currentGrade = 0;

    public void Upgrade()
    {
        switch(StructureType)
        {
            case (int)STRUCTURE_TYPE.STAT_UPGRADE:
                var stat = GetComponent<StatUpgradeBuilding>();
                if (StatType == (int)stat.upgradeStat)
                {
                    stat.upgradeValue = StatReturn;
                    ++currentGrade;
                    stat.RiseStat();
                }
                break;
            case (int)STRUCTURE_TYPE.PARAMETER_RECOVERY:
                var parameter = GetComponent<ParameterRecoveryBuilding>();
                if((PARAMETER_TYPES)ParameterType == parameter.parameterType)
                {
                    parameter.recoveryAmount += ParameterRecovery;
                    parameter.recoveryTime = RecvoeryTime;
                    ++currentGrade;
                }
                break;
            case (int)STRUCTURE_TYPE.ITEM_PRODUCE:
                break;
            case (int)STRUCTURE_TYPE.ITEM_SELL:
                break;
        }
    }

}
