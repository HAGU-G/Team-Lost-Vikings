﻿using UnityEngine;

public enum STAT_GROUP
{
    UNIT_ON_VILLAGE,
    UNIT_ON_DUNGEON,
    MONSTER
}


[System.Serializable]
public abstract class Stats
{
    [field: SerializeField] public int Id { get; protected set; }
    [field: SerializeField] public string Name { get; protected set; }

    //Parameters
    [field: SerializeField] public Parameter HP { get; set; } = new();

    //Stats
    [field: SerializeField] public StatFloat MoveSpeed { get; protected set; } = new();
    [field: SerializeField] public StatFloat UnitSize { get; protected set; } = new();
    [field: SerializeField] public StatFloat RecognizeRange { get; protected set; } = new();
    [field: SerializeField] public StatFloat PresenseRange { get; protected set; } = new();
    [field: SerializeField] public StatFloat AttackRange { get; protected set; } = new();
    [field: SerializeField] public StatFloat AttackSpeed { get; protected set; } = new();
    [field: SerializeField] public int CombatPoint { get; protected set; }

    public Transform transform = null;
    public Ellipse SizeEllipse { get; set; } = null;
    public Ellipse RecognizeEllipse { get; set; } = null;
    public Ellipse PresenseEllipse { get; set; } = null;
    public Ellipse BasicAttackEllipse { get; set; } = null;

    public float AttackTimer { get; set; }


    //Methods
    public void InitEllipses(Transform transform)
    {
        this.transform = transform;
        var pos = transform.position;
        SizeEllipse = new(UnitSize.Current, pos);
        RecognizeEllipse = new(RecognizeRange.Current, pos);
        PresenseEllipse = new(PresenseRange.Current, pos);
        BasicAttackEllipse = new(AttackRange.Current, pos);
    }

    public void ResetEllipses()
    {
        if (transform == null)
            return; 

        var pos = transform.position;
        SizeEllipse.SetAxies(UnitSize.Current, pos);
        RecognizeEllipse.SetAxies(RecognizeRange.Current, pos);
        PresenseEllipse.SetAxies(PresenseRange.Current, pos);
        BasicAttackEllipse.SetAxies(AttackRange.Current, pos);
    }

    public void UpdateEllipses()
    {
        if (transform == null)
            return;

        var pos = transform.position;
        SizeEllipse.position = pos;
        RecognizeEllipse.position = pos;
        PresenseEllipse.position = pos;
        BasicAttackEllipse.position = pos;
    }

    public void UpdateAttackTimer()
    {
        if (AttackTimer < AttackSpeed.Current)
        {
            AttackTimer += Time.deltaTime;
        }
    }

    public void Collision(Stats other)
    {
        var collisionDepth = SizeEllipse.CollisionDepthWith(other.SizeEllipse);
        if (collisionDepth >= 0f)
        {
            transform.position -= (other.transform.position - transform.position).normalized * collisionDepth;
        }
    }


    public virtual void ResetStats()
    {
        HP.Reset();
    }

    /// <param name="correctionFunc">null일 경우 내림 처리</param>
    public static int GetWeightedStat(int value, float weight, System.Func<float, int> correctionFunc = null)
    {
        correctionFunc ??= Mathf.FloorToInt;

        return correctionFunc(value * weight);
    }
}
