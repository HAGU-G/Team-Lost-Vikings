public class ItemData : ITableAvaialable<int>
{
    public string Name { get; set; }
    public int CurrencyId { get; set; }
    public string CurrencyAssetFileName { get; set; }

    public int TableID => CurrencyId;
}