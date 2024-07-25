using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

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
            _gold = Mathf.Clamp(value, 0, 100000000);
            GameManager.uiManager.uiDevelop.SetGold(value);
        }
    }
    [JsonProperty] public int Rune { get; set; }

    [JsonProperty] public Dictionary<int, int> ownItemList = new();
}
