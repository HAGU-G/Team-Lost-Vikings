using UnityEngine;

public class AttackOnHunt : State<UnitOnHunt>
{


    public override void EnterState()
    {
        owner.currentState = UnitOnHunt.STATE.ATTACK;
        owner.isTargetFixed = true;
        owner.isActing = true;

        owner.LookTarget(owner.attackTarget.transform);
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