public class AttackMonster : State<Monster>
{
    public override void EnterState()
    {
        owner.currentState = Monster.STATE.ATTACK;
        owner.isActing = true;

        owner.LookAt(owner.attackTarget.transform);
        owner.TryAttack();
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

        if (Transition())
            return;
    }

    protected override bool Transition()
    {
        if (owner.stats.AttackTimer < owner.stats.AttackSpeed.Current)
            return false;

        if (!owner.HasTarget())
        {
                controller.ChangeState((int)Monster.STATE.IDLE);
            return true;
        }

        controller.ChangeState((int)Monster.STATE.TRACE);

        return true;
    }
}