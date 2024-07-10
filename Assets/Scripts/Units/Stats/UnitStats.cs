using UnityEngine;

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

[System.Serializable]
public class UnitStats
{
    /// <summary>
    /// defaultStats가 null이면 GachaStats(data)합니다.
    /// </summary>
    /// <param name="defaultStats">얕은 복사. UnitStatsVariable 객체의 Clone() 메서드로 깊은 복사가 가능합니다.</param>
    public UnitStats(UnitStatsData data, UnitStatsVariable defaultStats = null)
    {
        Init(data, defaultStats);
        ResetStats();
    }

    //Save
    public int Id { get; private set; }
    public UNIT_GRADE UnitGrade { get; private set; }
    [field: SerializeField] public UnitStatsVariable DefaultStats { get; private set; }
    [field: SerializeField] public UnitStatsVariable CurrentStats { get; private set; }
    public int SkillId1 { get; private set; }
    public int SkillId2 { get; private set; }

    private int _currentHP;
    private int _currentStamina;
    private int _currentStress;

    //Don't Save
    public string Name { get; private set; }
    public UNIT_JOB UnitJob { get; private set; }
    public ATTACK_TYPE BasicAttackType { get; private set; }

    public int CurrentMaxHP { get; private set; }
    public int CurrentHP
    {
        get => _currentHP;
        set
        {
            _currentHP = Mathf.Clamp(value, 0, CurrentMaxHP);
        }
    }

    public int CurrentMaxStamina => CurrentStats.MaxStamina;
    public int CurrentStamina
    {
        get => _currentStamina;
        set
        {
            _currentStamina = Mathf.Clamp(value, 0, CurrentStats.MaxStamina);
        }
    }

    public int CurrentMaxStress => CurrentStats.MaxStress;
    public int CurrentStress
    {
        get => _currentStress;
        set
        {
            _currentStress = Mathf.Clamp(value, 0, CurrentStats.MaxStress);
        }
    }
    public float HPRatio => (float)CurrentHP / CurrentMaxHP;
    public float StaminaRatio => (float)CurrentStamina / CurrentStats.MaxStamina;
    public float StressRatio => (float)CurrentStress / CurrentStats.MaxStress;


    //Methods
    public void Init(UnitStatsData data, UnitStatsVariable defaultStats = null)
    {
        if (defaultStats == null)
            SetDefaultStats(data);
        else
            SetDefaultStats(defaultStats);

        SetConstantStats(data.Name, data.Job, data.BasicAttackType);

        //TODO 스킬도 GachaStats에서 뽑도록 변경해야함
        SkillId1 = data.SkillId1;
        SkillId2 = data.SkillId2;

        DefaultStats.UpdateCombatPoint();
    }

    public void SetConstantStats(string name, UNIT_JOB job, ATTACK_TYPE basicAttackType)
    {
        Name = name;
        UnitJob = job;
        BasicAttackType = basicAttackType;
    }

    public void SetDefaultStats(UnitStatsData data)
    {
        DefaultStats = GachaStats(data);
    }

    /// <summary>
    /// 얕은 복사. UnitStatsVariable 객체의 Clone() 메서드로 깊은 복사가 가능합니다.
    /// </summary>
    public void SetDefaultStats(UnitStatsVariable defaultStats)
    {
        DefaultStats = defaultStats;
    }

    public void ResetStats()
    {
        CurrentStats = DefaultStats.Clone();
        CurrentStats.UpdateCombatPoint();

        UpdateMaxHP();
        CurrentHP = CurrentMaxHP;

        CurrentStress = CurrentStats.MaxStress;
        CurrentStamina = CurrentStats.MaxStamina;
    }

    private void UpdateMaxHP()
    {
        CurrentMaxHP = CurrentStats.BaseHP + StatMath.GetWeightedStat(CurrentStats.Vit, CurrentStats.VitWeight);

        if (CurrentHP > CurrentMaxHP)
            CurrentHP = CurrentMaxHP;
    }

    public static UnitStatsVariable GachaStats(UnitStatsData data)
    {
        return new UnitStatsVariable()
        {
            MaxStress = data.Stress,
            MaxStamina = data.Stamina,

            BaseHP = data.HP,
            Vit = Random.Range(data.VitMin, data.VitMax + 1),
            VitWeight = data.VitWeight,

            Str = Random.Range(data.StrMin, data.StrMax + 1),
            StrWeight = data.StrWeight,
            Mag = Random.Range(data.MagMin, data.MagMax + 1),
            MagWeight = data.MagWeight,
            Agi = Random.Range(data.AgiMin, data.AgiMax + 1),
            AgiWeight = data.AgiWeight,

            UnitSize = data.SizeRange,
            MoveSpeed = data.MoveSpeed,
            RecognizeRange = data.RecognizeRange,

            AttackRange = data.BasicAttackRange,
            AttackSpeed = data.AttackSpeed,

            CriticalChance = data.CriticalChance,
            CriticalWeight = data.CriticalHitWeight
        };
    }
}
