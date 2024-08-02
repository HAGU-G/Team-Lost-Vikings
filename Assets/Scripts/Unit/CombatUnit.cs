using System.Collections.Generic;
using UnityEngine;

public abstract class CombatUnit : Unit, IDamagedable, IAttackable
{
    public HuntZone CurrentHuntZone { get; protected set; } = null;
    public Vector3 PortalPos { get; protected set; }

    protected IAttackStrategy attackBehaviour = new AttackDefault();

    public List<CombatUnit> Enemies { get; protected set; }
    public CombatUnit attackTarget;
    public bool isTargetFixed;

    public abstract bool IsNeedReturn { get; }
    public bool forceReturn = false;


    public enum STATE
    {
        IDLE,
        TRACE,
        ATTACK,
        SKILL,
        DEAD,
        RETURN,
    }
    public FSM<CombatUnit> FSM { get; private set; }
    public STATE currentState;

    public event System.Action OnDamaged;
    public event System.Action OnAttacked;
    public event System.Action OnUpdated;





    public override void Init()
    {
        base.Init();

        FSM = new();
        FSM.Init(this, 0,
            new IdleOnHunt(),
            new TraceOnHunt(),
            new AttackOnHunt(),
            new UseSkillOnHunt(),
            new DeadOnHunt(),
            new ReturnOnHunt()
            );
    }

    protected override void ResetEvents()
    {
        base.ResetEvents();
        OnDamaged = null;
        OnAttacked = null;
        OnUpdated = null;
    }
    public override void OnRelease()
    {
        base.OnRelease();
        CurrentHuntZone = null;
    }




    private void Update()
    {
        if (stats == null || gameObject.activeSelf == false)
            return;

        stats.UpdateAttackTimer();

        OnUpdated?.Invoke();

        stats.UpdateEllipsePosition();
        FSM.Update();

        //FSM 이후에 복귀나 사망으로 stats가 변할 수 있으므로 재검사
        if (stats == null || gameObject.activeSelf == false)
            return;

        stats.Collision(CurrentHuntZone.gridMap, CurrentHuntZone.Units.ToArray());
        stats.Collision(CurrentHuntZone.gridMap, CurrentHuntZone.Monsters.ToArray());
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

    public bool TakeDamage(int damage, ATTACK_TYPE type)
    {
        float def = type switch
        {
            ATTACK_TYPE.PHYSICAL => stats.PhysicalDef.Current,
            ATTACK_TYPE.MAGIC => stats.MagicalDef.Current,
            ATTACK_TYPE.SPECIAL => stats.SpecialDef.Current,
            _ => 0
        };
        var calculatedDamage = Mathf.FloorToInt(damage * (1f - def / 100f));

        stats.HP.Current -= calculatedDamage;
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

        animator?.AnimAttack(stats.Data.BasicAttackMotion);
        OnAttacked?.Invoke();
        stats.AttackTimer = 0f;

        stats.Stamina.Current -= GameSetting.Instance.staminaReduceAmount;
        isTargetFixed = true;


        return true;
    }

    public void ForceChangeTarget(CombatUnit enemy)
    {
        if (enemy == null
            || !enemy.gameObject.activeSelf
            || !Enemies.Contains(enemy))
            return;

        attackTarget = enemy;
        FSM.ChangeState((int)STATE.TRACE);
    }

    protected override void OnAnimationAttackHit()
    {
        if (!HasTarget())
        {
            isTargetFixed = false;
            return;
        }

        base.OnAnimationAttackHit();


        if (!attackTarget.isTargetFixed)
        {
            attackTarget.isTargetFixed = true;
            attackTarget.attackTarget = this;
        }

        bool isCritical = Random.Range(0, 100) < stats.CritChance.Current;
        var criticalWeight = isCritical ? stats.CritWeight.Current : 1f;
        var damage = Mathf.FloorToInt(stats.CombatPoint * criticalWeight);

        if (attackBehaviour.Attack(attackTarget, damage, stats.Data.BasicAttackType))
        {
            attackTarget = null;
            stats.Stress.Current -= GameSetting.Instance.stressReduceAmount;
        }
    }
}