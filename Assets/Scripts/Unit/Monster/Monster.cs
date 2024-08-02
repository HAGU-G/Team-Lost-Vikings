using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Monster : Unit, IDamagedable, ISubject<Monster>, IAttackable
{
    public MonsterStats stats = new();
    public override Stats GetStats => stats;
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


    public override void Init()
    {
        fsm = new();
        fsm.Init(this, 0,
            new IdleMonster(),
            new TraceMonster(),
            new AttackMonster(),
            new DeadMonster());
    }

    public void ResetMonster(HuntZone huntZone, bool isBoss = false)
    {
        CurrentHuntZone = huntZone;

        ResetEvents();
        stats.InitStats(isBoss ? huntZone.GetCurrentBoss() : huntZone.GetCurrentMonster());
        stats.ResetStats();
        stats.isBoss = isBoss;
        attackTarget = null;

        ResetBase();

        Enemies = CurrentHuntZone.Units;

        fsm.ResetFSM();
    }

    protected override void OnAnimationAttackHit() 
    {
        if (!HasTarget())
            return;

        base.OnAnimationAttackHit();


        if (!attackTarget.isTargetFixed)
        {
            attackTarget.isTargetFixed = true;
            attackTarget.attackTarget = this;
        }

        bool isCritical = Random.Range(0, 100) < stats.CritChance.Current;
        var criticalWeight = isCritical ? stats.CritWeight.Current : 1f;
        var damage = Mathf.FloorToInt(stats.CombatPoint * criticalWeight);

        if (attackBehaviour.Attack(attackTarget, stats.CombatPoint))
        {
            attackTarget = null;
        }
    }

    //public void UpdateAnimator()
    //{
    //    if (!isActing && animator != null && dress != null)
    //    {
    //        if (transform.position != prePos)
    //        {
    //            float preLook = Mathf.Sign(dress.transform.localScale.x);
    //            float currLook = Mathf.Sign((transform.position - prePos).x) * -1f;
    //            bool flip = (preLook != currLook) && currLook != 0f;

    //            dress.transform.localScale = new Vector3(
    //                dress.transform.localScale.x * (flip ? -1f : 1f),
    //                dress.transform.localScale.y,
    //                dress.transform.localScale.z);

    //            animator.AnimRun();
    //        }
    //        else
    //        {
    //            animator.AnimIdle();
    //        }
    //    }
    //    prePos = transform.position;
    //}

    public override void OnRelease()
    {
        base.OnRelease();
        CurrentHuntZone = null;
    }


    private void Update()
    {
        stats.UpdateAttackTimer();
        stats.UpdateEllipsePosition();
        fsm.Update();


        if (stats != null)
        {
            stats.Collision(CurrentHuntZone.gridMap, CurrentHuntZone.Units.ToArray());
            stats.Collision(CurrentHuntZone.gridMap, CurrentHuntZone.Monsters.ToArray());
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

    public bool TryAttack()
    {
        if (!HasTarget())
            return false;

        animator?.AnimAttack(stats.Data.BasicAttackMotion);

        stats.AttackTimer = 0f;
        return true;
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

    public void SendNotification(NOTIFY_TYPE type, bool removeObservers = false)
    {
        if (observer.Count == 0)
            return;

        for (int i = observer.Count - 1; i >= 0; i--)
        {
            observer[i].ReceiveNotification(this, type);
            if (removeObservers)
                observer.RemoveAt(i);

        }
    }

    public override void RemoveUnit()
    {
        base.RemoveUnit();
        SendNotification(NOTIFY_TYPE.REMOVE, true);
        GameManager.huntZoneManager.ReleaseMonster(this);
    }

    public void DropItem()
    {
        if (stats.Data.DropId == 0)
            return;

        var dropData = DataTableManager.dropTable.GetData(stats.Data.DropId);
        var itemList = GameManager.itemManager.ownItemList;

        GameManager.itemManager.Gold += dropData.DropGold();
        foreach (var itemID in dropData.DropItem())
        {
            if (itemList.ContainsKey(itemID))
            {
                itemList[itemID]++;
            }
            else
            {
                itemList.Add(itemID, 1);
            }
        }
    }
}
