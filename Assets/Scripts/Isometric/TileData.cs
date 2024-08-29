public class TileData : ITableAvaialable<int>
{
    public int Id { get; set; }
    public int Xindex { get; set; }
    public int Yindex { get; set; }
    public string TownFileName { get; set; }
    public string OutlandFileName { get; set; }

    public int TableID => Id;
}