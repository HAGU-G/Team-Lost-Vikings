using System;
using UnityEngine;

[Serializable]
public class UnitStats
{
    #region DEFAULT_STATS
    public UNIT.GROUP unitGroup;
    public UNIT.CLASS classType;
    public int defaultMaxHP;
    public int defaultAttackDamage;
    public float defaultAttackRange;
    public float defaultAttackInterval;
    #endregion

    #region FOR_CALCULATE
    private int _maxHP;
    private int _currentHP;
    private int _attackDamage;
    private float _attackRange;
    private float _attackInterval;
    #endregion

    #region CURRENT_STATS
    public int MaxHP 
    {
        get => _maxHP;
        private set => _maxHP = value; 
    }

    public int CurrentHP
    {
        get => _currentHP;
        set
        {
            _currentHP = Mathf.Clamp(value, 0, MaxHP);
        }
    }

    public int AttackDamage
    {
        get => _attackDamage;
        private set => _attackDamage = value;
    }

    public float AttackRange
    {
        get => _attackRange;
        private set => _attackRange = value;
    }

    public float AttackInterval
    {
        get => _attackInterval;
        private set => _attackInterval = value;
    }
    #endregion

    public void ResetStats()
    {
        CurrentHP = MaxHP = defaultMaxHP;

        AttackDamage = defaultAttackDamage;
        AttackRange = defaultAttackRange;
        AttackInterval = defaultAttackInterval;
    }
}
