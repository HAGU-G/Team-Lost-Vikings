using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.UI.CanvasScaler;

public class Monster : MonoBehaviour, IDamagedable, ISubject<Monster>, IAttackable, IStatUsable
{
    public MonsterStatsData testData;
    public MonsterStats stats = new();
    public Stats GetStats => stats;
    public STAT_GROUP StatGroup => STAT_GROUP.MONSTER;

    public HuntZone CurrentHuntZone { get; private set; } = null;

    public enum STATE
    {
        IDLE,
        TRACE,
        ATTACK,
        DEAD
    }

    private FSM<Monster> fsm;
    public STATE currentState;

    //Attack
    private IAttackStrategy attackBehaviour = new AttackDefault();
    public UnitOnHunt attackTarget;
    public List<UnitOnHunt> Enemies { get; private set; }

    public List<IObserver<Monster>> attackers = new();

    //AdditionalStats
    public bool IsDead { get; private set; }

    public void Ready(HuntZone huntZone = null)
    {
        Init(huntZone);
        ResetMonster();
    }
    public void Init(HuntZone huntZone = null)
    {
        CurrentHuntZone = huntZone;

        stats.InitStats(testData);
        stats.InitEllipse(transform);

        fsm = new();
        fsm.Init(this, 0,
            new IdleMonster(),
            new TraceMonster(),
            new AttackMonster(),
            new DeadMonster());
    }

    public void ResetMonster()
    {
        ResetEvents();
        stats.ResetStats();
        stats.ResetEllipse();

        IsDead = false;

        Enemies = CurrentHuntZone?.Units;

        fsm.ResetFSM();
    }

    public void ResetEvents() { }


    private void Update()
    {
        stats.UpdateAttackTimer();
        stats.UpdateEllipsePosition();
        fsm.Update();
        CollisionUpdate();
    }
    private void CollisionUpdate()
    {
        if (CurrentHuntZone == null)
            return;

        var units = CurrentHuntZone.Units;
        var monsters = CurrentHuntZone.Monsters;
        var maxCount = Mathf.Max(units.Count, monsters.Count);

        for (int i = 0; i < maxCount; i++)
        {
            if(i < units.Count && units[i] != this)
            {
                stats.Collision(units[i].stats);
            }

            if (i < monsters.Count && monsters[i] != this)
            {
                stats.Collision(monsters[i].stats);
            }
        }
    }

    public void Subscribe(IObserver<Monster> observer)
    {
        if (attackers.Contains(observer))
            return;

        attackers.Add(observer);
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

        if (!IsDead && stats.HP.Current <= 0)
        {
            IsDead = true;
            fsm.ChangeState((int)STATE.DEAD);
            return true;
        }

        return false;
    }

    public void UnSubscrive(IObserver<Monster> observer)
    {
        if (!attackers.Contains(observer))
            return;

        attackers.Remove(observer);
    }

    public int TryAttack()
    {
        if (attackTarget == null)
            return -1;

        stats.AttackTimer = 0f;

        if (attackBehaviour.Attack(attackTarget, stats.CombatPoint))
            return 1;

        return 0;
    }
}
