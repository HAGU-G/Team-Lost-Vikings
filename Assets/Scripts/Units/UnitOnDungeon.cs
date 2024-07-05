using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitOnDungeon : Unit, IDamagedable
{
    #region INSPECTOR
    public SpriteRenderer spriteRenderer;
    #endregion

    public enum STATE
    {
        IDLE,
        TRACE,
        ATTACK
    }

    private FSM<UnitOnDungeon> dungeonFSM;
    public STATE currentState;

    private IAttackStrategy attackBehaviour = new AttackDefault();
    public UnitOnDungeon attackTarget;
    public List<UnitOnDungeon> Targets {get; private set;}

    public DungeonManager dungeonManager;

    #region EVENT
    public event Action OnDamaged;
    #endregion

    private void Awake()
    {
        Init();
        ResetUnit();
    }

    private void Start()
    {
        if (stats.unitGroup == UNIT.GROUP.PLAYER)
            Targets = dungeonManager.monsters;
        else
            Targets = dungeonManager.players;
    }

    protected override void Init()
    {
        base.Init();

        dungeonFSM = new();
        dungeonFSM.Init(this, 0,
            new IdleOnDungeon(),
            new TraceOnDungeon(),
            new AttackOnDungeon());
    }

    protected override void ResetUnit()
    {
        base.ResetUnit();
        dungeonFSM.ResetFSM();
    }

    public int TakeDamage(int damage)
    {
        var preHP = stats.CurrentHP;

        stats.CurrentHP -= damage;
        OnDamaged?.Invoke();

        if (stats.CurrentHP == 0)
        {
            dungeonManager.unitCount--;
            Destroy(gameObject);
            return -1;
        }
        return preHP - stats.CurrentHP;
    }

    /// <returns>공격 실패시 -1 반환</returns>
    public int TryAttack()
    {
        if (attackTarget == null)
            return -1;

        return attackBehaviour.Attack(stats.AttackDamage, attackTarget);
    }

    private void Update()
    {
        dungeonFSM.Update();
    }
}
