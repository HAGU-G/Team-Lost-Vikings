public class DeadOnHunt : State<UnitOnHunt>
{
    public override void EnterState()
    {
        owner.currentState = UnitOnHunt.STATE.DEAD;
        owner.ReturnToVillage();
        owner.isActing = true;
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