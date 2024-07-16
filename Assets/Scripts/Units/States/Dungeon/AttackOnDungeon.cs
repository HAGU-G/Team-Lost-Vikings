public class AttackOnDungeon : State<UnitOnHunt>
{


    public override void EnterState()
    {
        owner.currentState = UnitOnHunt.STATE.ATTACK;
        //owner.spriteRenderer.color = Color.red;
    }

    public override void ExitState()
    {
    }

    public override void ResetState()
    {
    }

    public override void Update()
    {
        owner.TryAttack();

        if (Transition())
            return;

    }

    protected override bool Transition()
    {
        if (owner.attackTarget == null)
        {
            if (owner.IsNeedReturn)
                controller.ChangeState((int)UnitOnHunt.STATE.RETURN);
            else
                controller.ChangeState((int)UnitOnHunt.STATE.IDLE);
        }

        controller.ChangeState((int)UnitOnHunt.STATE.TRACE);

        return true;
    }
}