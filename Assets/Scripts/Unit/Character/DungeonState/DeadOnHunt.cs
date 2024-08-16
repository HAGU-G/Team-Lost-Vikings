using UnityEngine;

public class DeadOnHunt : State<CombatUnit>
{
    private float deathTimer = 0f;
    public override void EnterState()
    {
        owner.isActing = true;
        owner.currentState = CombatUnit.STATE.DEAD;
        owner.animator.AnimDeath();

        switch (owner.stats.Data.UnitType)
        {
            case UNIT_TYPE.CHARACTER:
                owner.stats.SetLocation(LOCATION.NONE, LOCATION.VILLAGE);
                break;
            case UNIT_TYPE.MONSTER:
                var monster = owner as Monster;
                monster.SendNotification(NOTIFY_TYPE.DEAD, true);
                monster.DropItem();
                break;
        }
    }

    public override void ExitState()
    {
        owner.isActing = false;
        deathTimer = 0f;
    }

    public override void ResetState()
    {
        owner.isActing = false;
        deathTimer = 0f;
    }

    public override void Update()
    {
        if (deathTimer < 1f)
        {
            deathTimer += Time.deltaTime;
            return;
        }

        switch (owner.stats.Data.UnitType)
        {
            case UNIT_TYPE.CHARACTER:
                owner.RemoveUnit();
                break;
            case UNIT_TYPE.MONSTER:
                var monster = owner as Monster;
                GameManager.huntZoneManager.ReleaseMonster(monster);
                break;
            default:
                break;
        }
    }

    protected override bool Transition()
    {
        return false;
    }
}