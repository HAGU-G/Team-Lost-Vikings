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
            if (!ownItemList.TryGetValue(0, out int gold))
            {
                ownItemList.Add(0, 0);
                return ownItemList[0];
            }
            return ownItemList[0];
        }
        set
        {
            if (!ownItemList.TryGetValue(0, out int gold))
            {
                ownItemList.Add(0, 0);
                ownItemList[0] = value;
            }

            if (value >= goldLimit)
            {
                if (ownItemList[0] < goldLimit)
                {
                    ownItemList[0] = goldLimit;
                }
            }
            else
            {
                ownItemList[0] = value;
            }
    

            GameManager.uiManager.uiDevelop.SetGold(ownItemList[0]);
        }
    }
    [JsonProperty] public int Rune { get; set; }

    [JsonProperty] public Dictionary<int, int> ownItemList = new();
    public int goldLimit = 4000;

    public bool AddGold(int amount)
    {
        if(!ownItemList.TryGetValue(0, out int gold))
        {
            ownItemList.Add(0, 0);
        }
        
        if (ownItemList[0] + amount >= goldLimit)
        {
            if (ownItemList[0] < goldLimit)
            {
                ownItemList[0] = goldLimit;
            }
            return false;
        }

        ownItemList[0] += amount;
        return true;
    }

    public bool SpendGold(int amount)
    {
        if (!ownItemList.TryGetValue(0, out int gold))
        {
            ownItemList.Add(0, 0);
        }

        if (ownItemList[0] - amount < 0)
        {
            return false;
        }

        ownItemList[0] -= amount;
        return true;
    }
}
