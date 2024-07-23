using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public enum UNIT_JOB
{
    NONE,
    WARRIOR,
    MAGICIAN,
    ARCHER
}

public enum ATTACK_TYPE
{
    NONE,
    PHYSICAL,
    MAGIC,
    SPECIAL,
}

public enum UNIT_GRADE
{
    COMMON,
    NORMAL,
    RARE,
    SUPER_RARE,
    ULTRA_RARE
}

public enum LOCATION
{
    NONE,
    VILLAGE,
    HUNTZONE
}

[System.Serializable, JsonObject(MemberSerialization.OptIn)]
public class UnitStats : Stats
{
    public static List<int> existIDs = new();

    [JsonProperty]
    [field: SerializeField]
    public int InstanceID { get; private set; }

    public UnitStats()
    {
        InstanceID = System.DateTime.Now.GetHashCode();
        while (existIDs.Contains(InstanceID))
        {
            InstanceID++;
        }
        existIDs.Add(InstanceID);
    }

    [JsonConstructor]
    public UnitStats(int instanceId)
    {
        InstanceID = instanceId;
        existIDs.Add(instanceId);
    }

    [JsonProperty] public UNIT_GRADE UnitGrade { get; private set; }
    public UNIT_JOB Job { get; private set; }
    public ATTACK_TYPE BasicAttackType { get; private set; }
    [JsonProperty] public int SkillId1 { get; private set; }
    [JsonProperty] public int SkillId2 { get; private set; }

    //Location
    [JsonProperty] public LOCATION Location { get; private set; }
    [JsonProperty] public LOCATION NextLocation { get; private set; }
    [JsonProperty][field: SerializeField] public int HuntZoneNum { get; private set; } = -1;

    [JsonProperty]
    [field: SerializeField]
    public Parameter Stamina { get; private set; } = new();

    [JsonProperty]
    [field: SerializeField]
    public Parameter Stress { get; private set; } = new();

    [JsonProperty]
    [field: SerializeField]
    public StatInt BaseHP { get; private set; } = new();

    [JsonProperty]
    [field: SerializeField]
    public StatInt Vit { get; private set; } = new();
    [field: SerializeField] public StatFloat VitWeight { get; private set; } = new();

    [JsonProperty]
    [field: SerializeField]
    public StatInt BaseStr { get; private set; } = new();
    [field: SerializeField] public StatFloat StrWeight { get; private set; } = new();

    [JsonProperty]
    [field: SerializeField]
    public StatInt BaseWiz { get; private set; } = new();
    [field: SerializeField] public StatFloat WizWeight { get; private set; } = new();

    [JsonProperty]
    [field: SerializeField]
    public StatInt BaseAgi { get; private set; } = new();
    [field: SerializeField] public StatFloat AgiWeight { get; private set; } = new();

    [field: SerializeField] public StatFloat CritChance { get; set; } = new();
    [field: SerializeField] public StatFloat CritWeight { get; set; } = new();


    public void InitStats(UnitStatsData data, bool doGacha = true)
    {
        if (doGacha)
            GachaDefaultStats(data);

        SetConstantStats(data);
        CalulateGrade();
    }

    public override void ResetStats()
    {
        UpdateMaxHP();

        base.ResetStats();
        Stress.Reset();
        Stamina.Reset();

        UpdateCombatPoint();
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

        return true;
    }

    public void UpdateCombatPoint()
    {
        CombatPoint =
            GetWeightedStat(BaseStr.Current, StrWeight.Current)
            + GetWeightedStat(BaseWiz.Current, WizWeight.Current)
            + GetWeightedStat(BaseAgi.Current, AgiWeight.Current);
    }
    public void GachaDefaultStats(UnitStatsData data)
    {
        Vit.defaultValue = Random.Range(data.VitMin, data.VitMax + 1);
        BaseStr.defaultValue = Random.Range(data.StrMin, data.StrMax + 1);
        BaseWiz.defaultValue = Random.Range(data.WizMin, data.WizMax + 1);
        BaseAgi.defaultValue = Random.Range(data.AgiMin, data.AgiMax + 1);
        SkillId1 = data.SkillPoolId1;
        SkillId2 = data.SkillPoolId2;
    }

    private void SetConstantStats(UnitStatsData data)
    {
        Id = data.Id;
        Name = data.Name;
        Job = data.Job;
        BasicAttackType = data.BasicAttackType;
        AssetFileName = data.UnitAssetFileName;

        //Stats
        Stamina.max = data.MaxStamina;
        Stamina.defaultValue = data.MaxStamina;

        Stress.max = data.MaxMental;
        Stress.defaultValue = data.MaxMental;

        BaseHP.defaultValue = data.MaxHP;

        VitWeight.defaultValue = data.VitWeight;
        StrWeight.defaultValue = data.StrWeight;
        WizWeight.defaultValue = data.WizWeight;
        AgiWeight.defaultValue = data.AgiWeight;

        UnitSize.defaultValue = data.SizeRange;
        MoveSpeed.defaultValue = data.MoveSpeed;
        RecognizeRange.defaultValue = data.RecognizeRange;
        PresenseRange.defaultValue = data.PresenseRange;

        AttackRange.defaultValue = data.BasicAttackRange;
        AttackSpeed.defaultValue = data.AttackSpeed;

        CritChance.defaultValue = data.CritChance;
        CritWeight.defaultValue = data.CritWeight;
    }

    public void UpdateMaxHP()
    {
        HP.max = BaseHP.Current + GetWeightedStat(Vit.Current, VitWeight.Current);
        HP.defaultValue = HP.max;

        if (HP.Current > HP.max)
            HP.Current = HP.max;
    }

    private void CalulateGrade()
    {
        //TODO 유닛 등급 계산 필요
        var overroll = BaseStr.defaultValue
            + BaseWiz.defaultValue
            + BaseAgi.defaultValue
            + Vit.defaultValue;

        UnitGrade = overroll switch
        {
            _ when (overroll >= 231) => UNIT_GRADE.ULTRA_RARE,
            _ when (overroll >= 121) => UNIT_GRADE.SUPER_RARE,
            _ when (overroll >= 91) => UNIT_GRADE.RARE,
            _ when (overroll >= 51) => UNIT_GRADE.NORMAL,
            _ => UNIT_GRADE.COMMON,
        };
    }


    public UnitStats Clone()
    {
        var clone = new UnitStats();
        clone.Id = Id;
        clone.Name = Name;
        clone.Job = Job;
        clone.HP = HP.Clone();
        clone.MoveSpeed = MoveSpeed.Clone() as StatFloat;
        clone.UnitSize = UnitSize.Clone() as StatFloat;
        clone.RecognizeRange = RecognizeRange.Clone() as StatFloat;
        clone.PresenseRange = PresenseRange.Clone() as StatFloat;
        clone.AttackRange = AttackRange.Clone() as StatFloat;
        clone.AttackSpeed = AttackSpeed.Clone() as StatFloat;

        clone.UnitGrade = UnitGrade;
        clone.Job = Job;
        clone.BasicAttackType = BasicAttackType;
        clone.SkillId1 = SkillId1;
        clone.SkillId2 = SkillId2;

        clone.Stamina = Stamina.Clone();
        clone.Stress = Stress.Clone();

        clone.BaseHP = BaseHP.Clone() as StatInt;
        clone.Vit = Vit.Clone() as StatInt;
        clone.VitWeight = VitWeight.Clone() as StatFloat;
        clone.BaseStr = BaseStr.Clone() as StatInt;
        clone.StrWeight = StrWeight.Clone() as StatFloat;
        clone.BaseWiz = BaseWiz.Clone() as StatInt;
        clone.WizWeight = WizWeight.Clone() as StatFloat;
        clone.BaseAgi = BaseAgi.Clone() as StatInt;
        clone.AgiWeight = AgiWeight.Clone() as StatFloat;

        clone.CritChance = CritChance.Clone() as StatFloat;
        clone.CritWeight = CritWeight.Clone() as StatFloat;

        return clone;
    }
}