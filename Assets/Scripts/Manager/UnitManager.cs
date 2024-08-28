using Newtonsoft.Json;
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

    public bool IsMaxWait => Waitings.Count >= GameSetting.Instance.waitListLimit;
    public bool CanGacha
    {
        get
        {
            if (IsMaxWait)
                return false;

            if (GetGachaPool(GameManager.playerManager.recruitLevel).Count <= 0)
                return false;

            return true;
        }
    }
    public int unitLimitCount = GameSetting.Instance.defaultUnitLimit;

    [JsonProperty] public System.DateTime lastAutoGachaTime;
    private float autoGachaTimeCorrection = 0f;
    public float TimeToAutoGacha
    {
        get
        {
            if (!GameManager.IsReady)
                return 8282f;

            float sec = (float)(SyncedTime.Now - lastAutoGachaTime).TotalSeconds;
            return Mathf.Max(0f, GameSetting.Instance.autoGachaSeconds - sec - autoGachaTimeCorrection);
        }
    }

    public void LoadUnits()
    {
        foreach (var unit in Units)
        {
            unit.Value.InitStats(DataTableManager.unitTable.GetData(unit.Value.Id), false);
            unit.Value.SetUpgradeStats();
            unit.Value.ResetMaxParameter();
        }
        foreach (var unit in DeadUnits)
        {
            unit.Value.InitStats(DataTableManager.unitTable.GetData(unit.Value.Id), false);
            unit.Value.SetUpgradeStats();
            unit.Value.ResetMaxParameter();
        }
        foreach (var unit in Waitings)
        {
            unit.Value.InitStats(DataTableManager.unitTable.GetData(unit.Value.Id), false);
            unit.Value.ResetMaxParameter();
            //unit.Value.SetUpgradeStats(); 하면 안됨. 대기목록에 있는 캐릭터는 업그레이드를 받지 않음
        }

        GameManager.Subscribe(EVENT_TYPE.START, OnGameStart);
        GameManager.Subscribe(EVENT_TYPE.GAME_READY, OnGameReady);
    }

    private void OnGameStart()
    {
        foreach (var unit in Units)
        {
            SpawnOnLocation(unit.Value);
        }
    }

    private void OnGameReady()
    {
        //누적된 가챠 진행
        float sleepSeconds = (float)(SyncedTime.Now - lastAutoGachaTime).TotalSeconds;
        var gachaCount = Mathf.FloorToInt(sleepSeconds / GameSetting.Instance.autoGachaSeconds);

        Debug.Log($"마지막 가챠로 부터 지난 시간: {sleepSeconds}, 누적된 가챠 수: {gachaCount}");

        while (gachaCount > 0 && CanGacha)
        {
            gachaCount--;
            GachaCharacter(GameManager.playerManager.recruitLevel);
        }
        autoGachaTimeCorrection = sleepSeconds - gachaCount * GameSetting.Instance.autoGachaSeconds;

        CoroutineObject.CreateCorutine(CoAutoGacha());
    }


    public UnitStats GetUnit(int instanceID)
    {
        if (Units.ContainsKey(instanceID))
            return Units[instanceID];
        if (Waitings.ContainsKey(instanceID))
            return Waitings[instanceID];
        if (DeadUnits.ContainsKey(instanceID))
            return DeadUnits[instanceID];

        Debug.LogWarning($"{instanceID} 존재하지 않는 유닛입니다.");
        return null;
    }

    public List<StatsData> GetGachaPool(int level)
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

        return gachaList;
    }

    private StatsData GachaUnitData(int level)
    {
        var gachaList = GetGachaPool(level);
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
                    (stats.isDead) ? STRUCTURE_TYPE.REVIVE : STRUCTURE_TYPE.PORTAL);
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
                GameManager.villageManager.village.UnitSpawn(stats.InstanceID,
                    (stats.isDead) ? STRUCTURE_TYPE.REVIVE : STRUCTURE_TYPE.PORTAL);
                break;
            case LOCATION.HUNTZONE:
                GameManager.huntZoneManager.HuntZones[stats.HuntZoneNum].SpawnUnit(stats.InstanceID);
                break;
        }
    }

    /// <param name="isIgnoreLimit">유닛 수 제한을 무시할 경우 true</param>
    public UnitStats GachaCharacter(int level, bool isIgnoreLimit = false)
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
        if ((!isDupli && Units.Count < unitLimitCount) || isIgnoreLimit)
            PickUpCharacter(waitCharacter.InstanceID);

        if (GameManager.IsReady)
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
            yield return null;

            if (SyncedTime.Now.AddSeconds(-(GameSetting.Instance.autoGachaSeconds - autoGachaTimeCorrection)) < lastAutoGachaTime)
                continue;

            if (!CanGacha)
            {
                autoGachaTimeCorrection = GameSetting.Instance.autoGachaSeconds - 1f;
                continue;
            }

            autoGachaTimeCorrection = 0f;
            if (GachaCharacter(GameManager.playerManager.recruitLevel) != null)
                lastAutoGachaTime = SyncedTime.Now;
            else
                autoGachaTimeCorrection = GameSetting.Instance.autoGachaSeconds - 1f;
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

    /// <returns>방출에 성공한 경우 드랍된 아이템 반환, 드랍된 아이템이 없거나 방출에 실패했을 경우 null 반환, Key: ID, Value: 아이템 개수 </returns>
    public Dictionary<int, int> DiscardCharacter(int instanceID)
    {
        UnitStats discardUnit = GetUnit(instanceID);

        if (discardUnit == null)
        {
            Debug.Log($"유닛 {instanceID}이(가) 존재하지 않습니다");
            return null;
        }

        Units.Remove(instanceID);
        Waitings.Remove(instanceID);
        DeadUnits.Remove(instanceID);

        discardUnit.ResetUpgradeStatsEvent();
        discardUnit.ResetHuntZone();
        if (discardUnit.objectTransform != null)
        {
            if (discardUnit.objectTransform.TryGetComponent<Unit>(out var unit))
                unit.RemoveUnit();
        }


        //아이템 드랍
        if (discardUnit.Data.DropId == 0)
            return null;

        var dropData = DataTableManager.dropTable.GetData(discardUnit.Data.DropId);
        var im = GameManager.itemManager;

        GameManager.playerManager.Exp += dropData.DropExp;
        Dictionary<int, int> dropList = new();
        foreach (var item in dropData.DropItem())
        {
            if (dropList.ContainsKey(item.Key))
                dropList[item.Key] += item.Value;
            else
                dropList.Add(item.Key, item.Value);
        }

        if (dropList.Count > 0)
            return dropList;
        else
            return null;
    }
}