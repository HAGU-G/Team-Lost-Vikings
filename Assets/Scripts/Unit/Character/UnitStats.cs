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
    PHYSICAL,
    MAGIC,
    SPECIAL,
    NONE
}

public enum UNIT_GRADE
{
    NORMAL,
    RARE,
    ULTRA_RARE,
    SUPER_RARE
}

public enum LOCATION
{
    NONE,
    VILLAGE,
    HUNTZONE
}

[System.Serializable]
public class UnitStats : Stats
{
    public static List<int> existIDs = new();
    [field: SerializeField] public int InstanceID { get; private set; }

    public UnitStats()
    {
        var newID = System.DateTime.Now.GetHashCode();

        while (existIDs.Contains(newID))
        {
            newID++;
        }

        InstanceID = newID;
        existIDs.Add(newID);
    }

    public UNIT_GRADE UnitGrade { get; private set; }
    public UNIT_JOB Job { get; private set; }
    public ATTACK_TYPE BasicAttackType { get; private set; }
    public int SkillId1 { get; private set; }
    public int SkillId2 { get; private set; }

    //Parameters
    public LOCATION Location { get; private set; }
    public LOCATION NextLocation { get; private set; }
    public int HuntZoneID { get; private set; } = -1;
    [field: SerializeField] public Parameter Stamina { get; private set; } = new();
    [field: SerializeField] public Parameter Stress { get; private set; } = new();

    //Stats
    [field: SerializeField] public StatInt BaseHP { get; private set; } = new();
    [field: SerializeField] public StatInt Vit { get; private set; } = new();
    [field: SerializeField] public StatFloat VitWeight { get; private set; } = new();
    [field: SerializeField] public StatInt Str { get; private set; } = new();
    [field: SerializeField] public StatFloat StrWeight { get; private set; } = new();
    [field: SerializeField] public StatInt Mag { get; private set; } = new();
    [field: SerializeField] public StatFloat MagWeight { get; private set; } = new();
    [field: SerializeField] public StatInt Agi { get; private set; } = new();
    [field: SerializeField] public StatFloat AgiWeight { get; private set; } = new();

    [field: SerializeField] public StatFloat CriticalChance { get; set; } = new();
    [field: SerializeField] public StatFloat CriticalWeight { get; set; } = new();


    public void InitStats(UnitStatsData data)
    {
        GachaDefaultStats(data);
        SetConstantStats(data);
    }

    public override void ResetStats()
    {
        SetMaxHP();
        base.ResetStats();

        Stress.Reset();
        Stamina.Reset();

        UpdateCombatPoint();
    }

    public void SetLocation(LOCATION location, LOCATION nextLocation = LOCATION.NONE)
    {
        Location = location;
        NextLocation = nextLocation;

        if (NextLocation != LOCATION.NONE)
            GameManager.unitManager.SpawnOnLocation(this);
    }

    public void SetHuntZone(int huntZoneID)
    {
        HuntZoneID = huntZoneID;
    }

    public void UpdateCombatPoint()
    {
        CombatPoint =
            GetWeightedStat(Str.Current, StrWeight.Current)
            + GetWeightedStat(Mag.Current, MagWeight.Current)
            + GetWeightedStat(Agi.Current, AgiWeight.Current);
    }
    public void GachaDefaultStats(UnitStatsData data)
    {
        Stamina.max = data.Stamina;
        Stamina.defaultValue = data.Stamina;
        Stress.max = data.Stress;
        Stress.defaultValue = data.Stress;

        BaseHP.defaultValue = data.HP;
        Vit.defaultValue = Random.Range(data.VitMin, data.VitMax + 1);
        VitWeight.defaultValue = data.VitWeight;
        SetMaxHP();
        HP.defaultValue = HP.max;

        Str.defaultValue = Random.Range(data.StrMin, data.StrMax + 1);
        StrWeight.defaultValue = data.StrWeight;
        Mag.defaultValue = Random.Range(data.MagMin, data.MagMax + 1);
        MagWeight.defaultValue = data.MagWeight;
        Agi.defaultValue = Random.Range(data.AgiMin, data.AgiMax + 1);
        AgiWeight.defaultValue = data.AgiWeight;

        UnitSize.defaultValue = data.SizeRange;
        MoveSpeed.defaultValue = data.MoveSpeed;
        RecognizeRange.defaultValue = data.RecognizeRange;

        AttackRange.defaultValue = data.BasicAttackRange;
        AttackSpeed.defaultValue = data.AttackSpeed;

        CriticalChance.defaultValue = data.CriticalChance;
        CriticalWeight.defaultValue = data.CriticalHitWeight;

        CalulateGrade();
        UpdateCombatPoint();
    }

    private static UnitStats GachaNewStats(UnitStatsData data)
    {
        var gacha = new UnitStats();
        gacha.GachaDefaultStats(data);


        return gacha;
    }

    private void SetConstantStats(UnitStatsData data)
    {
        Id = data.Id;
        Name = data.Name;
        Job = data.Job;
        BasicAttackType = data.BasicAttackType;
        AssetFileName = data.UnitAssetFileName;
    }

    private void SetMaxHP()
    {
        HP.max = BaseHP.Current + GetWeightedStat(Vit.Current, VitWeight.Current);
        HP.defaultValue = HP.max;

        if (HP.Current > HP.max)
            HP.Current = HP.max;
    }

    private void CalulateGrade()
    {
        //TODO 유닛 등급 계산 필요
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
        clone.Str = Str.Clone() as StatInt;
        clone.StrWeight = StrWeight.Clone() as StatFloat;
        clone.Mag = Mag.Clone() as StatInt;
        clone.MagWeight = MagWeight.Clone() as StatFloat;
        clone.Agi = Agi.Clone() as StatInt;
        clone.AgiWeight = AgiWeight.Clone() as StatFloat;

        clone.CriticalChance = CriticalChance.Clone() as StatFloat;
        clone.CriticalWeight = CriticalWeight.Clone() as StatFloat;

        return clone;
    }
}