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
    public UnitStats(UnitStatsData data)
    {
        SetConstantStats(data);
        ResetStats();
    }

    //Save
    public int Id { get; private set; }
    public UNIT_GRADE UnitGrade { get; private set; }
    [field: SerializeField] private UnitStatsVariable DefaultStats { get; set; }
    [field: SerializeField] public UnitStatsVariable CurrentStats { get; private set; }
    public int SkillId1 { get; private set; }
    public int SkillId2 { get; private set; }

    private int _currentHP;
    private int _currentStress;
    private int _currentStamina;

    //Don't Save
    public string Name { get; private set; }
    public UNIT_JOB UnitJob { get; private set; }
    public ATTACK_TYPE AttackType { get; private set; }
    public int CurrentMaxHP { get; private set; }
    public int CurrentHP
    { 
        get => _currentHP;
        set
        {
            _currentHP = Mathf.Clamp(value, 0, CurrentMaxHP);
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
    public int CurrentStamina 
    {
        get => _currentStamina;
        set
        {
            _currentStamina = Mathf.Clamp(value, 0, CurrentStats.MaxStamina);
        }
    }
    public float HPRatio => (float)CurrentHP / CurrentMaxHP;
    public float StressRatio => (float)CurrentStress / CurrentStats.MaxStress;
    public float StaminaRatio => (float)CurrentStamina / CurrentStats.MaxStamina;

    //Methods
    public void ResetStats()
    {
        CurrentStats = DefaultStats.Clone();
        CurrentStats.UpdateCombatPoint();

        UpdateMaxHP();
        CurrentHP = CurrentMaxHP;

        CurrentStress = CurrentStats.MaxStress;
        CurrentStamina = CurrentStats.MaxStamina;
    }

    public void SetConstantStats(UnitStatsData data)
    {
        DefaultStats = GachaStats(data);
        DefaultStats.UpdateCombatPoint();

        Name = data.Name;
        UnitJob = data.Job;
        AttackType = data.BasicAttackType;
        SkillId1 = data.SkillId1;
        SkillId2 = data.SkillId2;
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
