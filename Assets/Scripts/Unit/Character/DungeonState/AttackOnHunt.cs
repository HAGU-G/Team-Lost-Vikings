public class AttackOnHunt : State<CombatUnit>
{
    public override void EnterState()
    {
        owner.currentState = CombatUnit.STATE.ATTACK;
        owner.isTargetFixed = true;
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

        if (owner.forceReturn)
        {
            controller.ChangeState((int)CombatUnit.STATE.RETURN);
            return true;
        }

        if (!owner.HasTarget())
        {
            if (owner.IsNeedReturn)
                controller.ChangeState((int)CombatUnit.STATE.RETURN);
            else
                controller.ChangeState((int)CombatUnit.STATE.IDLE);
            return true;
        }

        controller.ChangeState((int)CombatUnit.STATE.TRACE);

        return true;
    }
}