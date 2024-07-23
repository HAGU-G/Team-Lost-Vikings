using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;

public enum DROP_TYPE
{
    ALL,
    ONE
}


public class DropData : ITableAvaialable<int>, ITableExtraLoadable
{
    public string DropName { get; set; }
    public int DropId { get; set; }
    public int MinGold { get; set; }
    public int MaxGold { get; set; }
    public DROP_TYPE DropType { get; set; }
    public List<int> DropChances { get; private set; } = new();
    public List<int> DropItemIds { get; private set; } = new();

    public int TableID => DropId;

    private static readonly string formatDropChance = "DropChance{0}";
    private static readonly string formatDropItemID = "DropItemId{0}";
    public void ExtraLoad(CsvReader reader)
    {
        int count = 1;

        while (true)
        {
            if (reader.TryGetField<int>(string.Format(formatDropChance, count), out var dropChance)
                && reader.TryGetField<int>(string.Format(formatDropItemID, count), out var dropID))
            {
                DropChances.Add(dropChance);
                DropItemIds.Add(dropID);
            }
            else
            {
                break;
            }

            count++;
        }
    }
}