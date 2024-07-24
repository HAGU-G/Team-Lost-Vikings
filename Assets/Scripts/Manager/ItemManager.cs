﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class ItemManager
{
    [JsonProperty] private int _gold;
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
