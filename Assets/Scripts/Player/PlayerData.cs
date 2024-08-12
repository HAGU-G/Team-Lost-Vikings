public class PlayerData : ITableAvaialable<int>
{
    public int PlayerLv { get; set; }
    public int Exp { get; set; }

    public int TableID => PlayerLv;
}