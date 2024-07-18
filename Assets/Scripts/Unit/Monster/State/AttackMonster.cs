public class AttackMonster : State<Monster>
{
    public override void EnterState()
    {
        owner.currentState = Monster.STATE.ATTACK;
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
        if (!owner.HasTarget())
            controller.ChangeState((int)UnitOnHunt.STATE.IDLE);

        controller.ChangeState((int)UnitOnHunt.STATE.TRACE);

        return true;
    }
}