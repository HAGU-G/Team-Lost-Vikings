﻿using CsvHelper;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeData : ITableAvaialable<int>, ITableExtraLoadable
{
    [field: SerializeField] public string UpgradeName { get; set; }
    [field: SerializeField] public int UpgradeId { get; set; }
    [field: SerializeField] public int UpgradeGrade { get; set; }
    [field: SerializeField] public int RequirePlayerLv { get; set; }
    [field: SerializeField] public STAT_TYPE StatType { get; set; }
    [field: SerializeField] public int StatReturn { get; set; }
    [field: SerializeField] public int ParameterType { get; set; }
    [field: SerializeField] public int ParameterRecovery { get; set; }
    [field: SerializeField] public float RecoveryTime { get; set; }
    [field: SerializeField] public int ProgressVarType { get; set; }
    [field: SerializeField] public float ProgressVarReturn { get; set; }
    [field: SerializeField] public int DropId { get; set; }
    [field: SerializeField] public List<int> ItemIds { get; private set; } = new();
    [field: SerializeField] public List<int> ItemNums { get; private set; } = new();
    [field: SerializeField] public string UpgradeDesc { get; set; }
    [field: SerializeField] public string StructureAssetFileName { get; set; }


    public int TableID => UpgradeId;
    private static readonly string formatItemID = "CurrencyId{0}";
    private static readonly string formatItemNum = "CurrencyNum{0}";
    public void ExtraLoad(CsvReader reader)
    {
        int count = 1;

        while (true)
        {
            if (reader.TryGetField<int>(string.Format(formatItemID, count), out var itemId)
                && reader.TryGetField<int>(string.Format(formatItemNum, count), out var itemNum))
            {
                ItemIds.Add(itemId);
                ItemNums.Add(itemNum);
            }
            else
            {
                break;
            }

            count++;
        }
    }


    public static UpgradeData GetUpgradeData(int upgradeID, int upgradeGrade)
    {
        foreach (var data in DataTableManager.upgradeTable.GetData(upgradeID))
        {
            if (data.UpgradeGrade == upgradeGrade)
            {
                return data;
            }
        }
        return null;
    }
}
