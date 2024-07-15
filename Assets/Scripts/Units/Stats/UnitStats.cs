
using System.Linq.Expressions;
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
    SPECIAL
}

public enum UNIT_GRADE
{
    NORMAL,
    RARE,
    ULTRA_RARE,
    SUPER_RARE
}

[System.Serializable]
public class UnitStats : Stats
{
    public UNIT_GRADE UnitGrade { get; private set; }
    public UNIT_JOB Job { get; private set; }
    public ATTACK_TYPE BasicAttackType { get; private set; }
    public int SkillId1 { get; private set; }
    public int SkillId2 { get; private set; }

    //Parameters
    public Parameter Stamina { get; set; } = new();
    public Parameter Stress { get; set; } = new();

    //Stats
    [field: SerializeField] public StatInt BaseHP { get; set; } = new();
    [field: SerializeField] public StatInt Vit { get; set; } = new();
    [field: SerializeField] public StatFloat VitWeight { get; set; } = new();
    [field: SerializeField] public StatInt Str { get; set; } = new();
    [field: SerializeField] public StatFloat StrWeight { get; set; } = new();
    [field: SerializeField] public StatInt Mag { get; set; } = new();
    [field: SerializeField] public StatFloat MagWeight { get; set; } = new();
    [field: SerializeField] public StatInt Agi { get; set; } = new();
    [field: SerializeField] public StatFloat AgiWeight { get; set; } = new();

    [field: SerializeField] public StatFloat CriticalChance { get; set; } = new();
    [field: SerializeField] public StatFloat CriticalWeight { get; set; } = new();

    public void Init(UnitStatsData data = null)
    {
        if (data != null)
            return;

        GachaDefaultStats(data);
        SetConstantStats(data);
    }

    public override void ResetStats()
    {
        Stress.Reset();
        Stamina.Reset();
        base.ResetStats();
    }

    public override void UpdateCombatPoint()
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
        Stress.defaultValue = data.Stamina;

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

    public static UnitStats GachaNewStats(UnitStatsData data)
    {
        var gacha = new UnitStats();
        gacha.GachaDefaultStats(data);

        return gacha;
    }

    protected void SetConstantStats(UnitStatsData data)
    {
        Name = data.Name;
        Job = data.Job;
        BasicAttackType = data.BasicAttackType;
    }

    protected override void SetMaxHP()
    {
        HP.max = BaseHP.Current + GetWeightedStat(Vit.Current, VitWeight.Current);

        if (HP.Current > HP.max)
            HP.Current = HP.max;
    }

    protected void CalulateGrade()
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
        clone.MoveSpeed = MoveSpeed.Clone();
        clone.UnitSize = UnitSize.Clone();
        clone.RecognizeRange = RecognizeRange.Clone();
        clone.PresenseRange = PresenseRange.Clone();
        clone.AttackRange = AttackRange.Clone();
        clone.AttackSpeed = AttackSpeed.Clone();

        clone.UnitGrade = UnitGrade;
        clone.Job = Job;
        clone.BasicAttackType = BasicAttackType;
        clone.SkillId1 = SkillId1;
        clone.SkillId2 = SkillId2;

        clone.Stamina = Stamina.Clone();
        clone.Stress = Stress.Clone();

        clone.BaseHP = BaseHP.Clone();
        clone.Vit = Vit.Clone();
        clone.VitWeight = VitWeight.Clone();
        clone.Str = Str.Clone();
        clone.StrWeight = StrWeight.Clone();
        clone.Mag = Mag.Clone();
        clone.MagWeight = MagWeight.Clone();
        clone.Agi = Agi.Clone();
        clone.AgiWeight = AgiWeight.Clone();

        clone.CriticalChance = CriticalChance.Clone();
        clone.CriticalWeight = CriticalWeight.Clone();

        return clone;
    }
}