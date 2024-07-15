public class IdleOnVillage : State<UnitOnVillage>
{
    public override void EnterState()
    {
        owner.currentState = UnitOnVillage.STATE.IDLE;

    }

    public override void ExitState()
    {
        
    }

    public override void ResetState()
    {
        
    }

    public override void Update()
    {
        if (Transition())
            return;
    }

    protected override bool Transition()
    {
        if(owner.CheckParameter() != UnitOnVillage.LACKING_PARAMETER.NONE)
            controller.ChangeState((int)UnitOnVillage.STATE.GOTO);

        return false;
    }
}