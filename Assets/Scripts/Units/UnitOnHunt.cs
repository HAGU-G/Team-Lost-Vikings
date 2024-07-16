using System.Collections.Generic;
using UnityEngine;

public class UnitOnHunt : Unit, IDamagedable, IObserver<Monster>, IAttackable
{
    //TESTCODE 던전 임시 연결
    public HuntZone dungeon;
    public Vector3 destinationPos;

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

    private FSM<UnitOnHunt> fsm;
    public STATE currentState;

    //Attack
    private IAttackStrategy attackBehaviour = new AttackDefault();
    public Monster attackTarget;
    public List<Monster> Enemies { get; private set; }

    private Dictionary<Monster, int> attackedTargets = new();

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
            return stats.HP.Ratio <= 0.1f
                || stats.Stamina.Ratio <= 0.5f
                || stats.Stress.Ratio <= 0.5f;
        }
    }


    private void Update()
    {
        stats.UpdateAttackTimer();

        OnUpdated?.Invoke();

        stats.UpdateEllipsePosition();
        fsm.Update();
        CollisionUpdate();
    }

    private void CollisionUpdate()
    {
        foreach (var unit in dungeon.Units)
        {
            if (unit == this)
                continue;
            stats.Collision(unit.stats);
        }

        foreach (var unit in dungeon.Monsters)
        {
            if (unit == this)
                continue;
            stats.Collision(unit.stats);
        }
        stats.UpdateEllipsePosition();
    }


    private void OnDestroy()
    {
        foreach (var target in attackedTargets)
        {
            target.Key.UnSubscrive(this);
        }
    }

    public void Ready()
    {
        Init();
        ResetUnit();
    }

    protected override void Init()
    {
        //base.Init();

        stats.InitEllipse(transform);

        fsm = new();
        fsm.Init(this, 0,
            new IdleOnDungeon(),
            new TraceOnDungeon(),
            new AttackOnDungeon(),
            new ReturnOnDungeon(),
            new UseSkillOnDungeon(),
            new DeadOnDungeon());


        //TESTCODE
        skills.SetSkills(0);
        skills.SetSkill(0, new Skill(testSkillData, this));
    }

    protected override void ResetUnit()
    {
        base.ResetUnit();

        IsDead = false;

        attackedTargets.Clear();
        Enemies = dungeon.Monsters;

        fsm.ResetFSM();
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

        attackTarget.Subscribe(this);
        StackStaminaToConsume(1, attackTarget);

        bool isCritical = Random.Range(0, 100) < stats.CriticalChance.Current;
        var criticalWeight = isCritical ? stats.CriticalWeight.Current : 1f;
        var damage = Mathf.FloorToInt(stats.CombatPoint * criticalWeight);

        if (attackBehaviour.Attack(attackTarget, damage, stats.BasicAttackType))
        {
            stats.Stress.Current--;
            return 1;
        }
        return 0;
    }

    private void StackStaminaToConsume(int stamina, Monster target)
    {
        if (attackedTargets.ContainsKey(target))
            attackedTargets[target] += stamina;
        else
            attackedTargets.Add(target, stamina);
    }

    private void ConsumeStamina(Monster target)
    {
        if (!attackedTargets.ContainsKey(target))
            return;

        stats.Stamina.Current -= attackedTargets[target];
        attackedTargets.Remove(target);
    }

    public void ReceiveNotification(Monster subject, NOTIFY_TYPE type = NOTIFY_TYPE.NONE)
    {
        if (type == NOTIFY_TYPE.DEAD)
            ConsumeStamina(subject);
    }
}