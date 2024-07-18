using UnityEngine;

[System.Serializable]
public class MonsterStats : Stats
{
    //Stats
    [field: SerializeField] public StatInt PhysicalDef { get; set; } = new();
    [field: SerializeField] public StatInt MagicalDef { get; set; } = new();
    [field: SerializeField] public StatInt SpecialDef { get; set; } = new();
    [field: SerializeField] public int DropId { get; set; }


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
    }

    private void SetConstantStats(MonsterStatsData data)
    {
        Id = data.MonsterId;
        Name = data.MonsterName;
        AssetFileName = data.MonsterAssetFileName;
    }

    public MonsterStats Clone()
    {
        var clone = new MonsterStats();

        clone.Id = Id;
        clone.Name = Name;

        clone.HP = HP.Clone();

        clone.MoveSpeed = MoveSpeed.Clone() as StatFloat;
        clone.UnitSize = UnitSize.Clone() as StatFloat;
        clone.RecognizeRange = RecognizeRange.Clone() as StatFloat;
        clone.PresenseRange = PresenseRange.Clone() as StatFloat;
        clone.AttackRange = AttackRange.Clone() as StatFloat;
        clone.AttackSpeed = AttackSpeed.Clone() as StatFloat;

        clone.PhysicalDef = PhysicalDef.Clone() as StatInt;
        clone.MagicalDef = MagicalDef.Clone() as StatInt;
        clone.SpecialDef = SpecialDef.Clone() as StatInt;
        clone.DropId = DropId;
        clone.AssetFileName = AssetFileName;

        return clone;
    }
}