using CsvHelper;
using UnityEngine;
using System.Collections.Generic;

public enum exp_TYPE
{
    ALL,
    ONE
}


public class DropData : ITableAvaialable<int>, ITableExtraLoadable
{
    public string DropName { get; set; }
    public int DropId { get; set; }
    public int DropExp { get; set; }
    public List<int> DropCurrenyIds { get; private set; } = new();
    public List<int> DropChances { get; private set; } = new();
    public List<int> DropMinNums { get; private set; } = new();
    public List<int> DropMaxNums { get; private set; } = new();

    public int TableID => DropId;

    private static readonly string formatDropItemID = "DropCurrencyId{0}";
    private static readonly string formatDropChance = "DropChance{0}";
    private static readonly string formatDropMin = "DropMinNum{0}";
    private static readonly string formatDropMax = "DropMaxNum{0}";
    public void ExtraLoad(CsvReader reader)
    {
        int count = 1;

        while (true)
        {
            if (reader.TryGetField<int>(string.Format(formatDropChance, count), out var dropChance)
                && reader.TryGetField<int>(string.Format(formatDropItemID, count), out var dropID)
                && reader.TryGetField<int>(string.Format(formatDropMin, count), out var dropMin)
                && reader.TryGetField<int>(string.Format(formatDropMax, count), out var dropMax))
            {
                if (dropID != 0)
                {
                    DropChances.Add(dropChance);
                    DropCurrenyIds.Add(dropID);
                    DropMinNums.Add(dropMin);
                    DropMaxNums.Add(dropMax);
                }
            }
            else
            {
                break;
            }

            count++;
        }
    }

    /// <summary>
    /// Key: 아이템ID, Value: 드롭량
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, int> DropItem()
    {
        Dictionary<int, int> result = new();
        var im = GameManager.itemManager;
        for (int i = 0; i < DropChances.Count; i++)
        {
            if (DropCurrenyIds[i] == 0)
                continue;

            if (Random.Range(0, 100) < DropChances[i])
            {
                int dropID = DropCurrenyIds[i];
                int dropAmount = Random.Range(DropMinNums[i], DropMaxNums[i] + 1);
                im.AddItem(dropID, dropAmount);

                if (result.ContainsKey(dropID))
                    result[dropID] += dropAmount;
                else
                    result.Add(dropID, dropAmount);
            }
        }
        return result;
    }
}