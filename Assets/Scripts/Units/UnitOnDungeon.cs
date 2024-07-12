using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UnitOnDungeon
    : Unit, IDamagedable, IObserver<UnitOnDungeon>, ISubject<UnitOnDungeon>
{
    #region INSPECTOR
    public SpriteRenderer spriteRenderer;
    #endregion

    public Vector3 destinationPos;

    //TESTCODE 던전 임시 연결
    public Dungeon dungeon;


    //State
    public enum STATE
    {
        IDLE,
        TRACE,
        ATTACK,
        RETURN,
        SKILL
    }

    private FSM<UnitOnDungeon> dungeonFSM;
    public UNIT_GROUP group;
    public STATE currentState;

    //Attack
    private IAttackStrategy attackBehaviour = new AttackDefault();
    public UnitOnDungeon attackTarget;
    public List<UnitOnDungeon> Enemies { get; private set; }

    public List<IObserver<UnitOnDungeon>> attackers = new();
    private Dictionary<UnitOnDungeon, int> attackedTargets = new();

    //Event
    public event Action OnDamaged;
    public event Action OnAttacked;
    public event Action OnUpdated;

    //AdditionalStats
    public float AttackTimer { get; private set; }
    private int staminaToConsume;
    public bool IsDead { get; private set; }
    public bool IsNeedReturn
    {
        get
        {
            return stats.HPRatio < 0.1f
                || stats.StaminaRatio < 0.5f
                || stats.StressRatio < 0.5f;
        }
    }

    private void Update()
    {
        if (AttackTimer < stats.CurrentStats.AttackSpeed)
        {
            AttackTimer += Time.deltaTime;
        }
        OnUpdated?.Invoke();

        dungeonFSM.Update();

    }

    private void OnDestroy()
    {
        foreach (var target in attackedTargets)
        {
            target.Key.UnSubscrive(this);
        }
    }

    // TESTCODE 소환 후 초기 설정함수
    // TODO Init와 ResetUnit을 public으로 변경할 예정
    public void Ready()
    {
        Init();
        ResetUnit();
    }

    protected override void Init()
    {
        //base.Init();

        dungeonFSM = new();
        dungeonFSM.Init(this, 0,
            new IdleOnDungeon(),
            new TraceOnDungeon(),
            new AttackOnDungeon(),
            new ReturnOnDungeon(),
            new UseSkillOnDungeon());

        //TESTCODE
        skills.SetSkills(0);
        skills.SetSkill(0, new Skill(testSkillData, this));
    }

    protected override void ResetUnit()
    {
        base.ResetUnit();

        OnDamaged = null;
        IsDead = false;

        attackedTargets.Clear();
        attackers.Clear();

        if (group == UNIT_GROUP.PLAYER)
            Enemies = dungeon.monsters;
        else
            Enemies = dungeon.players;

        dungeonFSM.ResetFSM();
    }

    protected override void ResetEvents()
    {
        base.ResetEvents();
        OnDamaged = null;
        OnAttacked = null;
        OnUpdated = null;
    }

    public bool TakeDamage(int damage)
    {
        stats.CurrentHP -= damage;
        OnDamaged?.Invoke();

        if (!IsDead && stats.CurrentHP <= 0)
        {
            Dead();
            return true;
        }

        return false;
    }

    public void Dead()
    {
        IsDead = true;
        foreach (var observer in attackers)
        {
            if (observer == null)
            {
                Debug.LogWarning($"{name}의 옵저버가 null입니다.");
                continue;
            }
            observer.ReceiveNotification(this);
        }
        Destroy(gameObject);
    }

    /// <returns>공격 실패: -1, 성공: 0, 이 공격으로 죽었을 경우 : 1</returns>
    public int TryAttack()
    {
        if (attackTarget == null)
            return -1;

        OnAttacked?.Invoke();

        attackTarget.Subscribe(this);
        StackStaminaToConsume(1, attackTarget);

        if (attackBehaviour.Attack(stats.CurrentStats.CombatPoint, attackTarget))
        {
            stats.CurrentStress--;
            return 1;
        }
        return 0;
    }

    public void StackStaminaToConsume(int stamina, UnitOnDungeon target)
    {
        if (attackedTargets.ContainsKey(target))
            attackedTargets[target] += stamina;
        else
            attackedTargets.Add(target, stamina);
    }

    private void ConsumeStamina(UnitOnDungeon target)
    {
        if (!attackedTargets.ContainsKey(target))
            return;

        stats.CurrentStamina -= attackedTargets[target];
        attackedTargets.Remove(target);
    }

    public void ReceiveNotification(UnitOnDungeon subject)
    {
        if (subject.IsDead)
            ConsumeStamina(subject);
    }

    public void Subscribe(IObserver<UnitOnDungeon> observer)
    {
        if (attackers.Contains(observer))
            return;

        attackers.Add(observer);
    }

    public void UnSubscrive(IObserver<UnitOnDungeon> observer)
    {
        if (!attackers.Contains(observer))
            return;

        attackers.Remove(observer);
    }
}