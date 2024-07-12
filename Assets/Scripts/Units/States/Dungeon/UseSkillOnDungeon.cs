using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class UseSkillOnDungeon : State<UnitOnDungeon>
{
    private float attackTimer;

    public override void EnterState()
    {
        owner.currentState = UnitOnDungeon.STATE.ATTACK;
        //owner.spriteRenderer.color = Color.magenta;
    }

    public override void ExitState()
    {
    }

    public override void ResetState()
    {
    }

    public override void Update()
    {
        if (owner.attackTarget == null && Transition())
            return;

        foreach (var skill in owner.skills.SkillList)
        {
            if (skill.IsReady
                && owner.attackTarget.hitCollider.IsCollidedWith(new(skill.Data.CastRange, owner.transform.position)))
            {
                skill.Use();
                break;
            }
        }

        if (Transition())
            return;

    }

    protected override bool Transition()
    {
        if (owner.attackTarget == null)
            controller.ChangeState((int)UnitOnDungeon.STATE.IDLE);
        else
            controller.ChangeState((int)UnitOnDungeon.STATE.TRACE);

        return true;
    }
}