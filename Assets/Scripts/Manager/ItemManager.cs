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
            _gold = Mathf.Clamp(value, 0, goldLimit);
            GameManager.uiManager.uiDevelop.SetGold(value);
        }
    }
    [JsonProperty] public int Rune { get; set; }

    [JsonProperty] public Dictionary<int, int> ownItemList = new();
    public int goldLimit = 4000;

    public bool AddGold(int amount)
    {
        if(Gold + amount > goldLimit)
        {
            Gold = goldLimit;
            return false;
        }

        Gold += amount;
        return true;
    }

    public bool SpendGold(int amount)
    {
        if (Gold - amount < 0)
        {
            return false;
        }

        Gold -= amount;
        return true;
    }
}
