﻿using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.BuildReportVisualizer;

[JsonObject(MemberSerialization.OptIn)]
public class PlayerManager
{
    [JsonProperty] public int level = 1;
    [JsonProperty] public int _exp;
    public int Exp 
    {
        get => _exp;
        set
        {
            _exp = value;
            var levelUpExp = DataTableManager.playerTable.GetData(level).Exp;
            while (_exp >= levelUpExp)
            {
                _exp -= levelUpExp;
                level++;
                levelUpExp = DataTableManager.playerTable.GetData(level).Exp;
            }
        }
    }

    [JsonProperty] public StatInt unitStr = new();
    [JsonProperty] public StatInt unitMag = new();
    [JsonProperty] public StatInt unitAgi = new();
    public StatFloat unitCritChance = new();
    public StatInt unitCritWeight = new();
    public StatInt unitHp = new();
    public StatInt unitStamina = new();
    public StatInt unitMental = new();

    public Dictionary<int, int> buildingUpgradeGrades = new(); //structureId, upgradeGrade
}