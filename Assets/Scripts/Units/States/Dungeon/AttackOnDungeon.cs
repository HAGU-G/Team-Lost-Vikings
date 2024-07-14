using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AttackOnDungeon : State<UnitOnDungeon>
{


    public override void EnterState()
    {
        owner.currentState = UnitOnDungeon.STATE.ATTACK;
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
                controller.ChangeState((int)UnitOnDungeon.STATE.RETURN);
            else
                controller.ChangeState((int)UnitOnDungeon.STATE.IDLE);
        }

        controller.ChangeState((int)UnitOnDungeon.STATE.TRACE);

        return true;
    }
}