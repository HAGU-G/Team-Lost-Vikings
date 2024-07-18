public class DeadOnHunt : State<UnitOnHunt>
{
    public override void EnterState()
    {
        owner.currentState = UnitOnHunt.STATE.DEAD;
        GameManager.huntZoneManager.ReleaseUnit(owner);
    }

    public override void ExitState()
    {
    }

    public override void ResetState()
    {
    }

    public override void Update()
    {
    }

    protected override bool Transition()
    {
        return false;
    }
}