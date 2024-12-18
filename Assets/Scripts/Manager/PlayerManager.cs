﻿using Newtonsoft.Json;
using System.Collections.Generic;

[JsonObject(MemberSerialization.OptIn)]
public class PlayerManager
{
    [JsonProperty] public int level = 1;
    [JsonProperty] public int _exp;
    public int prevLevel = 0;
    public int Exp
    {
        get => _exp;
        set
        {
            _exp = value;

            int levelUpExp;

            if (DataTableManager.playerTable.ContainsKey(level))
                levelUpExp = DataTableManager.playerTable.GetData(level).Exp;
            else
                return;

            GameManager.uiManager.uiDevelop.SetExpBar();
            while (_exp >= levelUpExp)
            {
                _exp -= levelUpExp;
                prevLevel = level;
                level++;
                GameManager.villageManager.LevelUp();
                GameManager.uiManager.uiDevelop.LevelUp();
                if (DataTableManager.playerTable.ContainsKey(level))
                    levelUpExp = DataTableManager.playerTable.GetData(level).Exp;
                else
                    break;
            }
        }
    }

    public int recruitLevel = 0;

    [JsonProperty] public bool firstPlay = true;

    [JsonProperty] public StatInt unitStr = new();
    [JsonProperty] public StatInt unitMag = new();
    [JsonProperty] public StatInt unitAgi = new();
    public StatFloat unitCritChance = new();
    public StatFloat unitCritWeight = new();
    public StatInt unitHp = new();
    public StatInt unitStamina = new();
    public StatInt unitMental = new();
    public StatFloat warriorWeight = new();
    public StatFloat magicianWeight = new();
    public StatFloat archerWeight = new();


    public Dictionary<int, int> buildingUpgradeGrades = new(); //structureId, upgradeGrade
}