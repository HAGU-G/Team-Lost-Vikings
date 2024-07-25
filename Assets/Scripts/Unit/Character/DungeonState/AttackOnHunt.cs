public class AttackOnHunt : State<UnitOnHunt>
{


    public override void EnterState()
    {
        owner.currentState = UnitOnHunt.STATE.ATTACK;
        //owner.
        //Renderer.color = Color.red;
    }

    public override void ExitState()
    {
    }

    public override void ResetState()
    {
    }

    public override void Update()
    {
        if (owner.HasTarget())
            owner.TryAttack();

        if (Transition())
            return;

    }

    protected override bool Transition()
    {
        if (owner.stats.AttackTimer < owner.stats.AttackSpeed.Current)
            return false;

        if (owner.forceReturn)
        {
            controller.ChangeState((int)UnitOnHunt.STATE.RETURN);
            return true;
        }

        if (!owner.HasTarget())
        {
            if (owner.IsNeedReturn)
                controller.ChangeState((int)UnitOnHunt.STATE.RETURN);
            else
                controller.ChangeState((int)UnitOnHunt.STATE.IDLE);
            return true;
        }

        controller.ChangeState((int)UnitOnHunt.STATE.TRACE);

        return true;
    }
}