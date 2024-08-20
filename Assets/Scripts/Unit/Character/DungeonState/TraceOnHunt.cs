using UnityEngine;

public class TraceOnHunt : State<CombatUnit>
{
    private bool isCollidedWithTarget;
    private float timer;
    public override void EnterState()
    {
        owner.currentState = CombatUnit.STATE.TRACE;
        timer = 0f;
        //owner.spriteRenderer.color = Color.yellow;
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
            owner.LookAt(owner.attackTarget.transform);
            owner.animator.AnimIdle();
            return;
        }

        var deltaTime = Time.deltaTime;
        timer += deltaTime;
        if (timer > GameSetting.Instance.traceRerouteTime)
        {
            owner.attackTarget = null;
            owner.isTargetFixed = true;
            timer = 0f;
        }
        else
        {
            owner.MoveToDestination(owner.attackTarget.transform, deltaTime);
        }
    }


    protected override bool Transition()
    {
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
        else
        {
            var stats = owner.stats;
            for (int i = 0; i < stats.Skills.Count; i++)
            {
                if (stats.Skills[i].IsReady
                    && owner.attackTarget.stats.SizeEllipse.IsCollidedWith(stats.Skills[i].CastEllipse))
                {
                    owner.usingSkillNum = i;
                    controller.ChangeState((int)CombatUnit.STATE.SKILL);
                    return true;
                }
            }

            isCollidedWithTarget = owner.attackTarget.stats.SizeEllipse.IsCollidedWith(stats.BasicAttackEllipse);

            if (stats.AttackTimer >= stats.AttackSpeed.Current && isCollidedWithTarget)
            {
                controller.ChangeState((int)CombatUnit.STATE.ATTACK);
                return true;
            }
        }

        return false;
    }
}