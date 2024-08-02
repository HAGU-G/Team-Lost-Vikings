public class DeadOnHunt : State<CombatUnit>
{
    public override void EnterState()
    {
        owner.isActing = true;
        owner.currentState = CombatUnit.STATE.DEAD;

        switch (owner.stats.Data.UnitType)
        {
            case UNIT_TYPE.CHARACTER:
                (owner as UnitOnHunt).ReturnToVillage();
                break;
            case UNIT_TYPE.MONSTER:
                var monster = owner as Monster;
                monster.SendNotification(NOTIFY_TYPE.DEAD, true);
                monster.DropItem();
                GameManager.huntZoneManager.ReleaseMonster(monster);
                break;
            default:
                break;
        }
    }

    public override void ExitState()
    {
        owner.isActing = false;
    }

    public override void ResetState()
    {
        owner.isActing = false;
    }

    public override void Update()
    {
    }

    protected override bool Transition()
    {
        return false;
    }
}