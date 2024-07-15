
using UnityEngine;

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
        owner.TryAttack();

        if (Transition())
            return;
    }

    protected override bool Transition()
    {
        if (owner.attackTarget == null)
            controller.ChangeState((int)UnitOnDungeon.STATE.IDLE);

        controller.ChangeState((int)UnitOnDungeon.STATE.TRACE);

        return true;
    }
}