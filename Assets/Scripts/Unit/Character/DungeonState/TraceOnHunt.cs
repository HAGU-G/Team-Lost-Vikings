using UnityEngine;

public class TraceOnHunt : State<CombatUnit>
{
    private bool isCollidedWithTarget;

    public override void EnterState()
    {
        owner.currentState = CombatUnit.STATE.TRACE;
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

        if(isCollidedWithTarget)
        {
            owner.LookAt(owner.attackTarget.transform);
            owner.animator.AnimIdle();
            return;
        }

        owner.MoveToDestination(owner.attackTarget.transform, Time.deltaTime);
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
            //스킬 사용 비활성화
            foreach (var skill in owner.skills.SkillList)
            {
                if (skill.IsReady
                    && owner.attackTarget.stats.SizeEllipse.IsCollidedWith(skill.CastEllipse))
                {
                    controller.ChangeState((int)CombatUnit.STATE.SKILL);
                    return true;
                }
            }

            isCollidedWithTarget = owner.attackTarget.stats.SizeEllipse.IsCollidedWith(owner.stats.BasicAttackEllipse);

            if (owner.stats.AttackTimer >= owner.stats.AttackSpeed.Current && isCollidedWithTarget)
            {
                controller.ChangeState((int)CombatUnit.STATE.ATTACK);
                return true;
            }
        }

        return false;
    }
}