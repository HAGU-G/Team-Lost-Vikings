﻿using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class UnitManager
{
    /// <summary>
    /// Key: instanceID,
    /// Value: UnitStats,
    /// 꼭 필요한 경우가 아니라면 GetUnit() 메서드를 사용해주세요.
    /// </summary>
    [JsonProperty] public Dictionary<int, UnitStats> Units { get; private set; } = new();
    [JsonProperty] public Dictionary<int, UnitStats> DeadUnits { get; private set; } = new();
    [JsonProperty] public Dictionary<int, UnitStats> Waitings { get; private set; } = new();

    public bool IsMaxWait => Waitings.Count >= GameSetting.Instance.autoGachaMaxCount;
    public int unitLimitCount = GameSetting.Instance.defaultUnitLimit;

    [JsonProperty] public System.DateTime lastAutoGachaTime = System.DateTime.Now;
    private float autoGachaTimeCorrection = 0f;
    public float TimeToAutoGacha
    {
        get
        {
            float sec = (System.DateTime.Now - lastAutoGachaTime).Seconds;
            float milliSec = (System.DateTime.Now - lastAutoGachaTime).Milliseconds * 0.001f;
            return Mathf.Max(0f, GameSetting.Instance.autoGachaSeconds - sec - milliSec);
        }
    }

    public void LoadUnits()
    {
        foreach (var unit in Units)
        {
            unit.Value.InitStats(DataTableManager.unitTable.GetData(unit.Value.Id), false);
            unit.Value.ResetMaxParameter();
            unit.Value.SetUpgradeStats();
        }
        foreach (var unit in DeadUnits)
        {
            unit.Value.InitStats(DataTableManager.unitTable.GetData(unit.Value.Id), false);
            unit.Value.ResetMaxParameter();
            unit.Value.SetUpgradeStats();
        }
        foreach (var unit in Waitings)
        {
            unit.Value.InitStats(DataTableManager.unitTable.GetData(unit.Value.Id), false);
            unit.Value.ResetMaxParameter();
            //unit.Value.SetUpgradeStats(); 하면 안됨. 대기목록에 있는 캐릭터는 업그레이드를 받지 않음
        }

        GameManager.Subscribe(EVENT_TYPE.START, OnGameStart);
    }

    private void OnGameStart()
    {
        foreach (var unit in Units)
        {
            SpawnOnLocation(unit.Value);
        }
        //누적된 가챠 진행
        var sleepSeconds = (System.DateTime.Now - lastAutoGachaTime).Seconds;
        var gachaCount = Mathf.FloorToInt(sleepSeconds / GameSetting.Instance.autoGachaSeconds);

        Debug.Log($"지난 시간: {sleepSeconds}, 누적된 가챠 수: {gachaCount}");

        while (gachaCount > 0 && !IsMaxWait)
        {
            gachaCount--;
            GachaCharacter(GameManager.playerManager.recruitLevel);
        }
        autoGachaTimeCorrection = sleepSeconds - gachaCount;



        CoroutineObject.CreateCorutine(CoAutoGacha());
    }


    public UnitStats GetUnit(int instanceID)
    {
        if (!Units.ContainsKey(instanceID))
        {
            Debug.LogWarning($"{instanceID} 존재하지 않는 유닛입니다.");
            return null;
        }

        return Units[instanceID];
    }

    private StatsData GachaUnitData(int level)
    {
        var gachaList = new List<StatsData>();

        foreach (var data in DataTableManager.unitTable.GetDatas())
        {
            if (data.UnitType != UNIT_TYPE.CHARACTER || level < data.GachaUnlockLv)
                continue;

            //중복검사
            bool isDupli = false;
            foreach (var unit in Waitings.Values)
            {
                if (unit.Id == data.Id)
                    isDupli = true;
            }
            if (isDupli)
                continue;

            for (int i = 0; i < data.GachaChance; i++)
            {
                gachaList.Add(data);
            }
        }
        StatsData result = null;
        if (gachaList.Count > 0)
            result = gachaList[Random.Range(0, gachaList.Count)];
        return result;
    }

    public void SpawnOnNextLocation(UnitStats stats)
    {
        if (stats.NextLocation == LOCATION.HUNTZONE
            && !GameManager.huntZoneManager.HuntZones.ContainsKey(stats.HuntZoneNum))
        {
            stats.SetLocation(LOCATION.NONE, LOCATION.VILLAGE);
            return;
        }

        switch (stats.NextLocation)
        {
            case LOCATION.NONE:
                break;
            case LOCATION.VILLAGE:
                GameManager.villageManager.village.UnitSpawn(stats.InstanceID,
                    (stats.HP.Current == 0) ? STRUCTURE_TYPE.REVIVE : STRUCTURE_TYPE.PORTAL);
                break;
            case LOCATION.HUNTZONE:
                GameManager.huntZoneManager.HuntZones[stats.HuntZoneNum].SpawnUnit(stats.InstanceID);
                break;
        }
    }
    public void SpawnOnLocation(UnitStats stats)
    {
        if (stats.Location == LOCATION.HUNTZONE
            && !GameManager.huntZoneManager.HuntZones.ContainsKey(stats.HuntZoneNum))
        {
            stats.SetLocation(LOCATION.NONE, LOCATION.VILLAGE);
            return;
        }
        switch (stats.Location)
        {
            case LOCATION.NONE:
                break;
            case LOCATION.VILLAGE:
                GameManager.villageManager.village.UnitSpawn(stats.InstanceID, STRUCTURE_TYPE.STANDARD);
                break;
            case LOCATION.HUNTZONE:
                GameManager.huntZoneManager.HuntZones[stats.HuntZoneNum].SpawnUnit(stats.InstanceID);
                break;
        }
    }

    public UnitStats GachaCharacter(int level)
    {
        var data = GachaUnitData(level);

        if (data == null)
            return null;

        var waitCharacter = new UnitStats();
        waitCharacter.InitStats(data);
        waitCharacter.ResetStats();
        Waitings.Add(waitCharacter.InstanceID, waitCharacter);

        //중복검사
        bool isDupli = false;
        foreach (var unit in Units.Values)
        {
            if (unit.Id == waitCharacter.Id)
                isDupli = true;
        }
        if (!isDupli)
            PickUpCharacter(waitCharacter.InstanceID);

        SaveManager.SaveGame();

        return waitCharacter;
    }

    public UnitStats PickUpCharacter(int instanceID)
    {
        var pick = Waitings[instanceID];
        Waitings.Remove(instanceID);

        if (Units.ContainsKey(pick.InstanceID))
            return null;

        Units.Add(pick.InstanceID, pick);
        pick.SetUpgradeStats();
        pick.SetLocation(LOCATION.VILLAGE);
        SpawnOnLocation(pick);

        SaveManager.SaveGame();

        return pick;
    }

    private IEnumerator CoAutoGacha()
    {
        while (true)
        {
            yield return new WaitForSeconds(GameSetting.Instance.autoGachaSeconds - autoGachaTimeCorrection);

            if (IsMaxWait)
            {
                autoGachaTimeCorrection = GameSetting.Instance.autoGachaSeconds - 1f;
            }
            else
            {
                autoGachaTimeCorrection = 0f;
                GachaCharacter(GameManager.playerManager.recruitLevel);
                lastAutoGachaTime = System.DateTime.Now;

                //TESTCODE
                (GameManager.uiManager.windows[WINDOW_NAME.CHARACTER_STASH] as UICharacterStash).LoadCharacterButtons(Waitings);
            }
        }
    }

    public void UnitUpgrade()
    {
        foreach (var unit in Units)
        {
            unit.Value.SetUpgradeStats();
        }
        foreach (var unit in DeadUnits)
        {
            unit.Value.SetUpgradeStats();
        }
    }

    public void SetUnitLimit(int value)
    {
        unitLimitCount = value;
    }
}