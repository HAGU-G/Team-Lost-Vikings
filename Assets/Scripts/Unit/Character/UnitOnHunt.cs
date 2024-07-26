using System.Collections.Generic;
using UnityEngine;

public class UnitOnHunt : Unit, IDamagedable, IAttackable
{
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
    public Vector3 PortalPos { get; private set; }

    public FSM<UnitOnHunt> FSM {get; private set;}
    public STATE currentState;
    public bool forceReturn = false;

    //Attack
    private IAttackStrategy attackBehaviour = new AttackDefault();
    public Monster attackTarget;
    public List<Monster> Enemies { get; private set; }

    //Event
    public event System.Action OnDamaged;
    public event System.Action OnAttacked;
    public event System.Action OnUpdated;

    //AdditionalStats
    public bool isTargetFixed;
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
        FSM.Update();
        UpdateAnimator();

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
            stats.Collision(unit.stats, CurrentHuntZone.gridMap);
        }

        foreach (var unit in CurrentHuntZone.Monsters)
        {
            if (unit == this)
                continue;
            stats.Collision(unit.stats, CurrentHuntZone.gridMap);
        }
    }


    public override void Init()
    {
        base.Init();

        FSM = new();
        FSM.Init(this, 0,
            new IdleOnHunt(),
            new TraceOnHunt(),
            new AttackOnHunt(),
            new ReturnOnHunt(),
            new UseSkillOnHunt(),
            new DeadOnHunt());
    }

    public void ResetUnit(UnitStats unitStats, HuntZone huntZone)
    {
        forceReturn = false;
        CurrentHuntZone = huntZone;
        PortalPos = CurrentHuntZone.PortalPos;
        ResetUnit(unitStats);
    }

    public override void ResetUnit(UnitStats unitStats)
    {
        base.ResetUnit(unitStats);
        unitStats.SetLocation(LOCATION.HUNTZONE);

        IsDead = false;

        attackTarget = null;
        isTargetFixed = false;
        Enemies = CurrentHuntZone.Monsters;

        FSM.ResetFSM();
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
            FSM.ChangeState((int)STATE.DEAD);
            return true;
        }

        return false;
    }

    public bool TryAttack()
    {
        if (!HasTarget())
            return false;

        animator?.AnimAttack();
        OnAttacked?.Invoke();
        stats.AttackTimer = 0f;

        stats.Stamina.Current -= GameSetting.Instance.staminaReduceAmount;
        isTargetFixed = true;


        return true;
    }

    protected override void OnAnimationAttackHit()
    {
        base.OnAnimationAttackHit();
        if (!HasTarget())
        {
            isTargetFixed = false;
            return;
        }

        bool isCritical = Random.Range(0, 100) < stats.CritChance.Current;
        var criticalWeight = isCritical ? stats.CritWeight.Current : 1f;
        var damage = Mathf.FloorToInt(stats.CombatPoint * criticalWeight);

        if (attackBehaviour.Attack(attackTarget, damage, stats.BasicAttackType))
        {
            attackTarget = null;
            stats.Stress.Current -= GameSetting.Instance.stressReduceAmount;
        }
    }

    public bool HasTarget()
    {
        if (attackTarget == null
            || !attackTarget.gameObject.activeSelf
            || !Enemies.Contains(attackTarget))
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
        FSM.ChangeState((int)STATE.TRACE);
    }
}