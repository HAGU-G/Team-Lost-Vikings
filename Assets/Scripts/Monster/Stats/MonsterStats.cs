using UnityEngine;

[System.Serializable]
public class MonsterStats : Stats
{
    //Stats
    [field: SerializeField] public StatInt PhysicalDef { get; set; } = new();
    [field: SerializeField] public StatInt MagicalDef { get; set; } = new();
    [field: SerializeField] public StatInt SpecialDef { get; set; } = new();
    [field: SerializeField] public int DropId { get; set; }
    [field: SerializeField] public string MonsterAssetFileName { get; set; }


    public void InitStats(MonsterStatsData data)
    {
        SetConstantStats(data);
        SetDefaultStats(data);
    }

    public void SetDefaultStats(MonsterStatsData data)
    {
        HP.max = data.MaxHP;
        HP.defaultValue = HP.max;

        UnitSize.defaultValue = data.SizeRange;
        MoveSpeed.defaultValue = data.MoveSpeed;
        RecognizeRange.defaultValue = data.RecognizeRange;

        AttackRange.defaultValue = data.BasicAttackRange;
        AttackSpeed.defaultValue = data.AttackSpeed;

        PhysicalDef.defaultValue = data.PhysicalDef;
        MagicalDef.defaultValue = data.MagicalDef;
        SpecialDef.defaultValue = data.SpecialDef;
        CombatPoint = data.CombatPoint;

        DropId = data.DropId;
        MonsterAssetFileName = data.MonsterAssetFileName;
    }

    private void SetConstantStats(MonsterStatsData data)
    {
        Id = data.MonsterId;
        Name = data.MonsterName;
    }

    public MonsterStats Clone()
    {
        var clone = new MonsterStats();

        clone.Id = Id;
        clone.Name = Name;

        clone.HP = HP.Clone();

        clone.MoveSpeed = MoveSpeed.Clone();
        clone.UnitSize = UnitSize.Clone();
        clone.RecognizeRange = RecognizeRange.Clone();
        clone.PresenseRange = PresenseRange.Clone();
        clone.AttackRange = AttackRange.Clone();
        clone.AttackSpeed = AttackSpeed.Clone();

        clone.PhysicalDef = PhysicalDef.Clone();
        clone.MagicalDef = MagicalDef.Clone();
        clone.SpecialDef = SpecialDef.Clone();
        clone.DropId = DropId;
        clone.MonsterAssetFileName = MonsterAssetFileName;

        return clone;
    }
}