using Newtonsoft.Json;
using System.Collections.Generic;

[JsonObject(MemberSerialization.OptIn)]
public class ItemManager
{
    [JsonProperty] private int _gold = 1000;
    public int Gold
    {
        get
        {
            return _gold;
        }
        set
        {
            _gold = value;
            GameManager.uiManager.uiDevelop.SetGold(value);
        }
    }
    [JsonProperty] public int Rune { get; set; }

    [JsonProperty] public Dictionary<int, int> ownItemList = new();
}
