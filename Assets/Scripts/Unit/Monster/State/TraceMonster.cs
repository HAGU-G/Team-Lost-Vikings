using UnityEngine;

public class TraceMonster : State<Monster>
{
    private bool isCollidedWithTarget;

    public override void EnterState()
    {
        owner.currentState = Monster.STATE.TRACE;
    }

    public override void ExitState()
    {
    }

    public override void ResetState()
    {
        isCollidedWithTarget = false;
    }

    public override void Update()
    {
        if (Transition())
            return;

        if (isCollidedWithTarget)
        {
            owner.LookTarget(owner.attackTarget.transform);
            return;
        }

        var moveDirection = owner.transform.position - (owner.attackTarget).transform.position;
        owner.transform.position -= moveDirection.normalized * Time.deltaTime * owner.stats.MoveSpeed.Current;
    }

    protected override bool Transition()
    {
        if (!owner.HasTarget())
        {
            controller.ChangeState((int)UnitOnHunt.STATE.IDLE);
            return true;
        }
        else
        {
            isCollidedWithTarget = owner.attackTarget.stats.SizeEllipse.IsCollidedWith(owner.stats.BasicAttackEllipse);

            if (owner.stats.AttackTimer >= owner.stats.AttackSpeed.Current && isCollidedWithTarget)
            {
                controller.ChangeState((int)UnitOnHunt.STATE.ATTACK);
                return true;
            }
        }

        return false;
    }
}