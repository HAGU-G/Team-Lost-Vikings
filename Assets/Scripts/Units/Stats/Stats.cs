using Unity.Jobs;
using UnityEngine;

public enum UNIT_GROUP
{
    PLAYER,
    MONSTER
}

[System.Serializable]
public abstract class Stats
{
    public int Id { get; protected set; }
    public string Name { get; protected set; }

    //Parameters
    public Parameter HP { get; set; } = new();

    //Stats
    [field: SerializeField] public StatFloat MoveSpeed { get; set; } = new();
    [field: SerializeField] public StatFloat UnitSize { get; set; } = new();
    [field: SerializeField] public StatFloat RecognizeRange { get; set; } = new();
    [field: SerializeField] public StatFloat PresenseRange { get; set; } = new();
    [field: SerializeField] public StatFloat AttackRange { get; set; } = new();
    [field: SerializeField] public StatFloat AttackSpeed { get; set; } = new();
    [field: SerializeField] public int CombatPoint { get; protected set; }


    //Methods

    public virtual void ResetStats()
    {
        SetMaxHP();
        HP.Reset();

        UpdateCombatPoint();
    }

    public abstract void UpdateCombatPoint();
    protected abstract void SetMaxHP();

    /// <param name="correctionFunc">null일 경우 내림 처리</param>
    public static int GetWeightedStat(int value, float weight, System.Func<float, int> correctionFunc = null)
    {
        correctionFunc ??= Mathf.FloorToInt;

        return correctionFunc(value * weight);
    }
}
