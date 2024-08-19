using System.Collections.Generic;
using UnityEngine;

public abstract class CombatUnit : Unit, IDamagedable, IAttackable, IHealedable
{
    public Transform damageEffectPosition;
    public HuntZone CurrentHuntZone { get; protected set; } = null;
    public Vector3 PortalPos { get; protected set; }

    protected IAttackStrategy attackBehaviour = null;

    public List<CombatUnit> Enemies { get; protected set; }
    public List<CombatUnit> Allies { get; protected set; }
    public CombatUnit attackTarget;
    public bool isTargetFixed;
    public int usingSkillNum = -1;

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

    public override void ResetUnit(UnitStats stats)
    {
        base.ResetUnit(stats);
        switch (stats.Data.BasicAttackMotion)
        {
            case ATTACK_MOTION.MAGIC:
            case ATTACK_MOTION.BOW:
                attackBehaviour = new AttackProjectile();
                var attackProjectile = attackBehaviour as AttackProjectile;
                attackProjectile.owner = stats;

                SkillData skill = new();
                attackProjectile.skill = skill;
                skill.SkillType = stats.Data.BasicAttackType;
                skill.SkillAttackType = SKILL_ATTACK_TYPE.PROJECTILE;
                skill.SkillTarget = TARGET_TYPE.ENEMY;
                switch (stats.Data.BasicAttackMotion)
                {
                    case ATTACK_MOTION.MAGIC:
                        skill.ProjectileFileName = "MagicProjectile";
                        skill.ProjectileSpeed = GameSetting.Instance.defaultMagicProjectileSpeed;
                        break;
                    default:
                        skill.ProjectileFileName = "BowProjectile";
                        skill.ProjectileSpeed = GameSetting.Instance.defaultBowProjectileSpeed;
                        break;
                };
                skill.SkillEffectName = string.Empty;
                break;
            default:
                attackBehaviour = new AttackDefault();
                break;
        }

        foreach (var skill in stats.Skills)
        {
            skill.ResetConditionUpdate();
        }
    }


    protected override void ResetEvents()
    {
        base.ResetEvents();
        OnDamaged = null;
        OnAttacked = null;
    }
    public override void OnRelease()
    {
        base.OnRelease();
        CurrentHuntZone = null;
    }




    protected override void Update()
    {
        if (stats == null || gameObject.activeSelf == false)
            return;

        base.Update();

        stats.UpdateEllipsePosition();
        FSM.Update();

        //FSM 이후에 복귀나 사망으로 stats가 변할 수 있으므로 재검사
        if (IsDead || gameObject.activeSelf == false)
            return;

        stats.Collision(CurrentHuntZone.gridMap, CurrentHuntZone.Units.ToArray());
        stats.Collision(CurrentHuntZone.gridMap, CurrentHuntZone.Monsters.ToArray());
    }






    public bool HasTarget()
    {
        if (attackTarget == null
            || !attackTarget.gameObject.activeSelf
            || attackTarget.IsDead
            || !Enemies.Contains(attackTarget))
        {
            attackTarget = null;
            return false;
        }

        return true;
    }

    public (bool, int) TakeDamage(int damage, ATTACK_TYPE type)
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
            animator?.AnimHit();
        
        OnDamaged?.Invoke();
        GameManager.effectManager.GetDamageEffect(
            calculatedDamage.ToString(),
            damageEffectPosition.position,
            Color.white);

        if (!IsDead && stats.HP.Current <= 0)
        {
            IsDead = true;
            FSM.ChangeState((int)STATE.DEAD);

            if (stats.Data.UnitType == UNIT_TYPE.MONSTER)
                GameManager.questManager.SetAchievementCountByTargetID(stats.Id, ACHIEVEMENT_TYPE.MONSTER_KILL, 1);
            return (true, calculatedDamage);
        }

        return (false, calculatedDamage);
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

    public List<(CombatUnit, float)> GetCollidedUnit(Ellipse ellipse, params CombatUnit[] units)
    {
        List<(CombatUnit, float)> targets = new();

        foreach (var target in units)
        {
            var depth = Ellipse.CollisionDepth(ellipse, target.stats.PresenseEllipse);
            if (depth >= 0f)
            {
                targets.Add((target, depth));
            }
        }

        targets.Sort((left, right) =>
            {
                return (int)Mathf.Sign(left.Item2 - right.Item2);
            });

        return targets;
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
            isTargetFixed = false;

        base.OnAnimationAttackHit();

        if (attackTarget != null && !attackTarget.isTargetFixed)
        {
            LookAt(attackTarget.transform);
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

    public void TakeHeal(int heal)
    {
        stats.HP.Current += heal;
        GameManager.effectManager.GetDamageEffect(
            heal.ToString(),
            damageEffectPosition.position,
            Color.green);
    }
}