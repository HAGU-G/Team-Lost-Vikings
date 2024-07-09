using UnityEngine;

public class TraceOnDungeon : State<UnitOnDungeon>
{
    public override void EnterState()
    {
        owner.currentState = UnitOnDungeon.STATE.TRACE;
        owner.spriteRenderer.color = Color.yellow;
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

        var moveDirection = owner.transform.position - (owner.attackTarget).transform.position;
        owner.transform.position -= moveDirection.normalized * Time.deltaTime * owner.stats.CurrentStats.MoveSpeed;
    }


    protected override bool Transition()
    {
        if (owner.attackTarget == null)
        {
            if (owner.IsNeedReturn)
                controller.ChangeState((int)UnitOnDungeon.STATE.RETURN);
            else
                controller.ChangeState((int)UnitOnDungeon.STATE.IDLE);
            return true;
        }
        else if (Vector3.Distance(owner.transform.position, owner.attackTarget.transform.position) <= owner.stats.CurrentStats.AttackRange)
        {
            controller.ChangeState((int)UnitOnDungeon.STATE.ATTACK);
            return true;
        }

        return false;
    }
}