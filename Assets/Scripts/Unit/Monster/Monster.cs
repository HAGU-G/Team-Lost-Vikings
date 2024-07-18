using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Monster : MonoBehaviour, IDamagedable, ISubject<Monster>, IAttackable, IStatUsable
{
    //TESTCODE
    public GameObject dress;

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

    public List<IObserver<Monster>> observer = new();

    //AdditionalStats
    public bool IsDead { get; private set; }

    public void Ready(HuntZone huntZone)
    {
        Init();
        ResetMonster(huntZone);
    }
    public void Init()
    {
        fsm = new();
        fsm.Init(this, 0,
            new IdleMonster(),
            new TraceMonster(),
            new AttackMonster(),
            new DeadMonster());
    }

    public void ResetMonster(HuntZone huntZone)
    {
        CurrentHuntZone = huntZone;

        //TODO 사냥터의 몬스터ID에 맞게 데이터 할당
        ResetEvents();
        stats.InitStats(huntZone.CurrentMonsterData);
        stats.ResetStats();
        stats.ResetEllipse(transform);

        //몬스터 외형 테스트 코드
        try
        {
            Addressables.InstantiateAsync(stats.AssetFileName, transform)
            .Completed += (handle) =>
            {
                if (dress != null)
                    Destroy(dress);

                dress = handle.Result;
            };
        }
        catch
        {
            Debug.LogWarning($"{stats.AssetFileName} 파일이 없습니다.");
            if (dress != null)
                Destroy(dress);
        }

        IsDead = false;

        Enemies = CurrentHuntZone.Units;

        fsm.ResetFSM();
    }

    public void OnRelease()
    {
        CurrentHuntZone = null;
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
        var units = CurrentHuntZone.Units;
        var monsters = CurrentHuntZone.Monsters;
        var maxCount = Mathf.Max(units.Count, monsters.Count);

        for (int i = 0; i < maxCount; i++)
        {
            if (i < units.Count && units[i] != this)
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
        if (this.observer.Contains(observer))
            return;

        this.observer.Add(observer);
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
        this.observer.Remove(observer);
    }

    public int TryAttack()
    {
        stats.AttackTimer = 0f;

        if (attackBehaviour.Attack(attackTarget, stats.CombatPoint))
            return 1;

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

    public void SendNotification(NOTIFY_TYPE type, bool removeObservers = false)
    {
        for (int i = observer.Count - 1; i >= 0; i--)
        {
            observer[i].ReceiveNotification(this, type);
            if (removeObservers)
                observer.RemoveAt(i);
        }
    }

    public void RemoveMonster()
    {
        SendNotification(NOTIFY_TYPE.REMOVE, true);
        GameManager.huntZoneManager.ReleaseMonster(this);
    }

    public void DropItem()
    {
        Debug.Log($"아이템 드롭 ID: {stats.DropId}");
    }
}
