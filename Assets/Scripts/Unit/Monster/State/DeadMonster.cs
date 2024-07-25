public class DeadMonster : State<Monster>
{
    public override void EnterState()
    {
        owner.currentState = Monster.STATE.DEAD;
        owner.SendNotification(NOTIFY_TYPE.DEAD, true);
        owner.DropItem();
        owner.isActing = true;
        GameManager.huntZoneManager.ReleaseMonster(owner);
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