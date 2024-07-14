using UnityEngine;
using UnityEngine.UIElements;

#region ENUM
public enum UNIT_GROUP
{
    PLAYER,
    MONSTER
}

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
#endregion

[System.Serializable]
public class UnitStats
{
    public int Id { get; private set; }
    public UNIT_GRADE UnitGrade { get; private set; }
    public string Name { get; private set; }
    public UNIT_JOB UnitJob { get; private set; }
    public ATTACK_TYPE BasicAttackType { get; private set; }

    //Parameters
    public Parameter HP { get; set; } = new();
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

    [field: SerializeField] public StatFloat UnitSize { get; set; } = new();
    [field: SerializeField] public StatFloat MoveSpeed { get; set; } = new();
    [field: SerializeField] public StatFloat RecognizeRange { get; set; } = new();

    [field: SerializeField] public StatFloat AttackRange { get; set; } = new();
    [field: SerializeField] public StatFloat AttackSpeed { get; set; } = new();

    [field: SerializeField] public StatFloat CriticalChance { get; set; } = new();
    [field: SerializeField] public StatFloat CriticalWeight { get; set; } = new();

    [field: SerializeField] public int CombatPoint { get; private set; }
    public int SkillId1 { get; private set; }
    public int SkillId2 { get; private set; }


    public UnitStats(UnitStatsData data = null)
    {
        Init(data);
        ResetUnitStats();
    }




    //Methods
    public void Init(UnitStatsData data)
    {

        if(data != null)
        SetDefaultStats(defaultStats);
        SetConstantStats(data.Name, data.Job, data.BasicAttackType);

        //TODO 스킬도 GachaStats에서 뽑도록 변경해야함
        SkillId1 = data.SkillId1;
        SkillId2 = data.SkillId2;

        DefaultStats.UpdateCombatPoint();
        CurrentStats = new();
    }

    public void ResetUnitStats()
    {
        UpdateUnitStats();

        HP = CurrentMaxHP;
        Stress = CurrentStats.MaxStress;
        Stamina = CurrentStats.MaxStamina;
    }

    public void UpdateUnitStats()
    {
        CurrentStats.ResetStats().AddStats(DefaultStats).AddStats(upgradeStats);
        CurrentStats.UpdateCombatPoint();

        UpdateMaxHP();
    }

    public void SetConstantStats(string name, UNIT_JOB job, ATTACK_TYPE basicAttackType)
    {
        Name = name;
        UnitJob = job;
        BasicAttackType = basicAttackType;
    }

    public void UpdateCombatPoint()
    {

    }

    public void GachaDefaultStats(UnitStatsData data)
    {
        Stress.max.defaultValue = data.Stress;
        Stamina.max.defaultValue = data.Stamina;

        BaseHP.defaultValue = data.HP;
        Vit.defaultValue = Random.Range(data.VitMin, data.VitMax + 1);
        VitWeight.defaultValue = data.VitWeight;

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

        UpdateCombatPoint();
    }
    public static UnitStats GachaNewStats(UnitStatsData data)
    {
        var gacha = new UnitStats();
        gacha.GachaDefaultStats(data);

        return gacha;
    }


    private void UpdateMaxHP()
    {
        MaxHP.defaultValue = BaseHP.defaultValue + StatMath.GetWeightedStat(Vit.defaultValue, VitWeight.defaultValue);
        if (HP > MaxHP)
            HP.Current = MaxHP.Value;
    }


}
