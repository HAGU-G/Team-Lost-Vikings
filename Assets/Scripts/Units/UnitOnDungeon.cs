using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

public class UnitOnDungeon : Unit, IDamagedable
{
    private IAttackStrategy attackBehaviour = new AttackDefault();
    public UnitOnDungeon attackTarget;
    public DungeonManager dungeonManager;
    private List<UnitOnDungeon> targets;

    public UNIT.STATE_ON_DUNGEON state;
    private FSM<UnitOnDungeon> dungeonFSM;
    public SpriteRenderer spriteRenderer;

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
        {
            targets = dungeonManager.monsters;
        }
        else
        {
            targets = dungeonManager.players;
        }
    }

    protected override void Init()
    {
        base.Init();

        dungeonFSM = new();
        dungeonFSM.Init(this, 0,
            gameObject.AddComponent<IdleOnDungeon>(),
            gameObject.AddComponent<TraceOnDungeon>(),
            gameObject.AddComponent<AttackOnDungeon>());
    }

    protected override void ResetUnit()
    {
        base.ResetUnit();

        dungeonFSM.ResetFSM();
    }

    public bool TakeDamage(int damage)
    {
        if (stats.CurrentHP == 0)
            return true;

        stats.CurrentHP -= damage;
        OnDamaged?.Invoke();

        if (stats.CurrentHP == 0)
        {
            dungeonManager.unitCount--;
            Destroy(gameObject);
            return true;
        }
        return false;
    }

    public void TryAttack()
    {
        if (attackTarget == null)
            return;

        if (attackBehaviour.Attack(stats.AttackDamage, attackTarget))
        {
            attackTarget = null;
        }
    }

    private void Update()
    {
        if (attackTarget != null)
        {
            if (state != UNIT.STATE_ON_DUNGEON.ATTACK)
            {
                if (state != UNIT.STATE_ON_DUNGEON.TRACE)
                {
                    dungeonFSM.ChangeState((int)UNIT.STATE_ON_DUNGEON.TRACE);
                }
                else if (Vector3.Distance(transform.position, attackTarget.transform.position) <= 1f)
                {
                    dungeonFSM.ChangeState((int)UNIT.STATE_ON_DUNGEON.ATTACK);
                }
            }
        }
        else if (state == UNIT.STATE_ON_DUNGEON.IDLE)
        {
            float min = float.MaxValue;
            foreach (var target in targets)
            {
                var d = Vector3.Distance(target.transform.position, transform.position);
                if (d < min)
                {
                    min = d;
                    attackTarget = target;
                }
            }

        }
        else
        {
            dungeonFSM.ChangeState((int)UNIT.STATE_ON_DUNGEON.IDLE);
        }

    }
}
