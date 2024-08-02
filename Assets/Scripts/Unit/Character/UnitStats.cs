using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public enum LOCATION
{
    NONE,
    VILLAGE,
    HUNTZONE
}

[System.Serializable, JsonObject(MemberSerialization.OptIn)]
public class UnitStats : Stats
{
    //Location
    [JsonProperty] public LOCATION Location { get; private set; }
    [JsonProperty] public LOCATION NextLocation { get; private set; }
    [JsonProperty][field: SerializeField] public int HuntZoneNum { get; private set; } = -1;

    public event System.Action ArriveVillage;
    public PARAMETER_TYPE parameterType;

    public override void InitStats(StatsData data, bool doGacha = true)
    {
        base.InitStats(data, doGacha);
        if (doGacha)
            RegisterCharacter();
    }

    public void SetUpgradeStats()
    {
        BaseStr.SetUpgrade(GameManager.playerManager.unitStr);
        BaseWiz.SetUpgrade(GameManager.playerManager.unitMag);
        BaseAgi.SetUpgrade(GameManager.playerManager.unitAgi);
    }

    public void SetLocation(LOCATION location, LOCATION nextLocation = LOCATION.NONE)
    {
        Location = location;
        NextLocation = nextLocation;

        if (Location == LOCATION.NONE && NextLocation != LOCATION.NONE)
            GameManager.unitManager.SpawnOnNextLocation(this);
    }

    public bool SetHuntZone(int huntZoneNum)
    {
        var hm = GameManager.huntZoneManager;
        var ud = hm.UnitDeployment;

        if (hm.IsDeployed(InstanceID, huntZoneNum)
            || ud[huntZoneNum].Count >= hm.HuntZones[huntZoneNum].GetCurrentData().UnitCapacity)
            return false;

        if (HuntZoneNum != huntZoneNum && HuntZoneNum != -1)
            ud[HuntZoneNum].Remove(InstanceID);

        ud[huntZoneNum].Add(InstanceID);
        HuntZoneNum = huntZoneNum;

        if (Location == LOCATION.HUNTZONE)
        {
            var unitOnHunt = objectTransform.GetComponent<UnitOnHunt>();
            if (unitOnHunt.CurrentHuntZone.HuntZoneNum == huntZoneNum)
            {
                unitOnHunt.forceReturn = false;
                unitOnHunt.FSM.ChangeState((int)UnitOnHunt.STATE.IDLE);
            }
            else
            {
                ForceReturn();
            }

        }

        return true;
    }

    public void ResetHuntZone()
    {
        if (HuntZoneNum == -1)
            return;

        GameManager.huntZoneManager.UnitDeployment[HuntZoneNum].Remove(InstanceID);
        HuntZoneNum = -1;

        if (Location == LOCATION.VILLAGE)
        {
            var unitOnVillage = objectTransform.GetComponent<UnitOnVillage>();
            if (unitOnVillage.currentState == UnitOnVillage.STATE.GOTO)
            {
                unitOnVillage.VillageFSM.ChangeState((int)UnitOnVillage.STATE.IDLE);
            }
        }
    }

    public void ForceReturn()
    {
        if (Location != LOCATION.HUNTZONE)
            return;

        objectTransform.GetComponent<UnitOnHunt>().forceReturn = true;
    }

    public void OnArrived()
    {
        ArriveVillage?.Invoke();
    }
}