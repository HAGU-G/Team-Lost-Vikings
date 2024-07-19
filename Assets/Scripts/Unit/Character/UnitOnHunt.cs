﻿using System.Collections.Generic;
using UnityEngine;

public class UnitOnHunt : Unit, IDamagedable, IAttackable
{
    //TESTCODE
    public Vector3 portalPos;

    //State
    public enum STATE
    {
        IDLE,
        TRACE,
        ATTACK,
        RETURN,
        SKILL,
        DEAD
    }
    public HuntZone CurrentHuntZone { get; private set; } = null;

    private FSM<UnitOnHunt> fsm;
    public STATE currentState;

    //Attack
    private IAttackStrategy attackBehaviour = new AttackDefault();
    public Monster attackTarget;
    public List<Monster> Enemies { get; private set; }

    //Event
    public event System.Action OnDamaged;
    public event System.Action OnAttacked;
    public event System.Action OnUpdated;

    //AdditionalStats
    public override STAT_GROUP StatGroup => STAT_GROUP.UNIT_ON_DUNGEON;
    public bool IsDead { get; private set; }
    public bool IsNeedReturn
    {
        get
        {
            return stats.HP.Ratio < GameSetting.Instance.returnHPRaito
                || stats.Stamina.Ratio < GameSetting.Instance.returnStaminaRaito
                || stats.Stress.Ratio < GameSetting.Instance.returnStressRaito;
        }
    }


    private void Update()
    {
        stats.UpdateAttackTimer();

        OnUpdated?.Invoke();

        stats.UpdateEllipsePosition();
        fsm.Update();

        //오브젝트가 더이상 사용하지 않는 상태인지 검사. TODO 개선 필요
        if (stats != null)
            CollisionUpdate();
    }

    private void CollisionUpdate()
    {
        stats.UpdateEllipsePosition();
        foreach (var unit in CurrentHuntZone.Units)
        {
            if (unit == this)
                continue;
            stats.Collision(unit.stats);
        }

        foreach (var unit in CurrentHuntZone.Monsters)
        {
            if (unit == this)
                continue;
            stats.Collision(unit.stats);
        }
    }


    public override void Init()
    {
        base.Init();

        fsm = new();
        fsm.Init(this, 0,
            new IdleOnHunt(),
            new TraceOnHunt(),
            new AttackOnHunt(),
            new ReturnOnHunt(),
            new UseSkillOnHunt(),
            new DeadOnHunt());
    }

    public void ResetUnit(UnitStats unitStats, HuntZone huntZone)
    {
        CurrentHuntZone = huntZone;
        portalPos = CurrentHuntZone.transform.position;
        ResetUnit(unitStats);
    }

    public override void ResetUnit(UnitStats unitStats)
    {
        base.ResetUnit(unitStats);
        unitStats.SetLocation(LOCATION.HUNTZONE);
        unitStats.SetHuntZone(CurrentHuntZone.HuntZoneID);

        IsDead = false;

        attackTarget = null;
        Enemies = CurrentHuntZone.Monsters;

        fsm.ResetFSM();
    }

    public override void OnRelease()
    {
        base.OnRelease();
        CurrentHuntZone = null;
    }

    public override void RemoveUnit()
    {
        base.RemoveUnit();
        GameManager.huntZoneManager.ReleaseUnit(this);
    }

    public void ReturnToVillage()
    {
        stats.SetLocation(LOCATION.NONE, LOCATION.VILLAGE);
        RemoveUnit();
    }

    protected override void ResetEvents()
    {
        base.ResetEvents();
        OnDamaged = null;
        OnAttacked = null;
        OnUpdated = null;
    }

    public bool TakeDamage(int damage, ATTACK_TYPE type)
    {
        stats.HP.Current -= damage;
        OnDamaged?.Invoke();

        if (!IsDead && stats.HP.Current <= 0)
        {
            IsDead = true;
            fsm.ChangeState((int)STATE.DEAD);
            return true;
        }

        return false;
    }

    public int TryAttack()
    {
        if (attackTarget == null)
            return -1;

        OnAttacked?.Invoke();
        stats.AttackTimer = 0f;

        stats.Stamina.Current -= GameSetting.Instance.staminaReduceAmount;

        bool isCritical = Random.Range(0, 100) < stats.CritChance.Current;
        var criticalWeight = isCritical ? stats.CritWeight.Current : 1f;
        var damage = Mathf.FloorToInt(stats.CombatPoint * criticalWeight);

        if (attackBehaviour.Attack(attackTarget, damage, stats.BasicAttackType))
        {
            stats.Stress.Current -= GameSetting.Instance.stressReduceAmount;
            return 1;
        }
        return 0;
    }

    public bool HasTarget()
    {
        if (attackTarget == null)
        {
            return false;
        }
        else if (!attackTarget.gameObject.activeSelf)
        {
            attackTarget = null;
            return false;
        }

        return true;
    }

    public void ForceChangeTarget(Monster monster)
    {
        if (monster == null
            || !monster.gameObject.activeSelf
            || !CurrentHuntZone.Monsters.Contains(monster))
            return;

        attackTarget = monster;
        fsm.ChangeState((int)STATE.TRACE);
    }
}