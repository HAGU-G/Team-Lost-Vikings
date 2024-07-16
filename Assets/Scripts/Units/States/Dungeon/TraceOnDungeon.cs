﻿using UnityEngine;

public class TraceOnDungeon : State<UnitOnHunt>
{
    private bool isCollidedWithTarget;

    public override void EnterState()
    {
        owner.currentState = UnitOnHunt.STATE.TRACE;
        //owner.spriteRenderer.color = Color.yellow;
    }

    public override void ExitState()
    {
    }

    public override void ResetState()
    {
    }

    public override void Update()
    {
        if (Transition() || isCollidedWithTarget)
            return;

        var moveDirection = owner.transform.position - (owner.attackTarget).transform.position;
        owner.transform.position -= moveDirection.normalized * Time.deltaTime * owner.stats.MoveSpeed.Current;

    }


    protected override bool Transition()
    {
        if (owner.attackTarget == null)
        {
            if (owner.IsNeedReturn)
                controller.ChangeState((int)UnitOnHunt.STATE.RETURN);
            else
                controller.ChangeState((int)UnitOnHunt.STATE.IDLE);
            return true;
        }
        else
        {
            foreach (var skill in owner.skills.SkillList)
            {
                if (skill.IsReady
                    && owner.attackTarget.stats.SizeEllipse.IsCollidedWith(skill.CastEllipse))
                {
                    controller.ChangeState((int)UnitOnHunt.STATE.SKILL);
                    return true;
                }
            }

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