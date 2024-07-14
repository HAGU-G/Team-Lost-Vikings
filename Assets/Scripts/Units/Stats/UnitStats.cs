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
    /// <param name="defaultStats">얕은 복사. UnitStatsVariable 객체의 Clone() 메서드로 깊은 복사가 가능합니다.</param>
    public UnitStats(UnitStatsData data, UnitStatsVariable defaultStats)
    {
        Init(data, defaultStats);
        ResetUnitStats();
    }
    public UnitStats(UnitStatsData data)
    {
        Init(data);
        ResetUnitStats();
    }


    //Save
    public int Id { get; private set; }
    public UNIT_GRADE UnitGrade { get; private set; }


    [field: SerializeField] public UnitStatsVariable DefaultStats { get; private set; }
    public int SkillId1 { get; private set; }
    public int SkillId2 { get; private set; }

    private int _currentHP;
    private int _currentStamina;
    private int _currentStress;

    //Don't Save
    public string Name { get; private set; }
    public UNIT_JOB UnitJob { get; private set; }
    public ATTACK_TYPE BasicAttackType { get; private set; }
    public static UnitStatsVariable upgradeStats = new();
    [field: SerializeField] public UnitStatsVariable CurrentStats { get; private set; }

    public int CurrentMaxHP { get; private set; }
    public int CurrentMaxStamina => CurrentStats.MaxStamina;
    public int CurrentMaxStress => CurrentStats.MaxStress;

    public int CurrentHP
    {
        get => _currentHP;
        set
        {
            _currentHP = Mathf.Clamp(value, 0, CurrentMaxHP);
        }
    }
    public int CurrentStamina
    {
        get => _currentStamina;
        set
        {
            _currentStamina = Mathf.Clamp(value, 0, CurrentStats.MaxStamina);
        }
    }
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

    /// <param name="defaultStats">얕은 복사. UnitStatsVariable 객체의 Clone() 메서드로 깊은 복사가 가능합니다.</param>
    public void Init(UnitStatsData data, UnitStatsVariable defaultStats)
    {
        SetDefaultStats(defaultStats);
        SetConstantStats(data.Name, data.Job, data.BasicAttackType);

        //TODO 스킬도 GachaStats에서 뽑도록 변경해야함
        SkillId1 = data.SkillId1;
        SkillId2 = data.SkillId2;

        DefaultStats.UpdateCombatPoint();
        CurrentStats = new();
    }
    public void Init(UnitStatsData data)
    {
        Init(data, GachaStats(data));
    }
    public void ResetUnitStats()
    {
        UpdateUnitStats();

        CurrentHP = CurrentMaxHP;
        CurrentStress = CurrentStats.MaxStress;
        CurrentStamina = CurrentStats.MaxStamina;
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

    public void GachaDefaultStats(UnitStatsData data)
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
