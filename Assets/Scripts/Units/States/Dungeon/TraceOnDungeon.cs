﻿using UnityEngine;

public class TraceOnDungeon : State<UnitOnDungeon>
{
    private bool isCollidedWithTarget;

    public override void EnterState()
    {
        owner.currentState = UnitOnDungeon.STATE.TRACE;
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
                controller.ChangeState((int)UnitOnDungeon.STATE.RETURN);
            else
                controller.ChangeState((int)UnitOnDungeon.STATE.IDLE);
            return true;
        }
        else
        {
            foreach (var skill in owner.skills.SkillList)
            {
                if (skill.IsReady
                    && owner.attackTarget.stats.SizeEllipse.IsCollidedWith(skill.CastEllipse))
                {
                    controller.ChangeState((int)UnitOnDungeon.STATE.SKILL);
                    return true;
                }
            }

            isCollidedWithTarget = owner.attackTarget.stats.SizeEllipse.IsCollidedWith(owner.stats.BasicAttackEllipse);

            if (owner.stats.AttackTimer >= owner.stats.AttackSpeed.Current && isCollidedWithTarget)
            {
                controller.ChangeState((int)UnitOnDungeon.STATE.ATTACK);
                return true;
            }
        }

        return false;
    }
}