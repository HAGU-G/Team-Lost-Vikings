public class UseSkillOnHunt : State<CombatUnit>
{
    public override void EnterState()
    {
        owner.currentState = CombatUnit.STATE.ATTACK;
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
        if (!owner.HasTarget() && Transition())
            return;

        foreach (var skill in owner.skills.SkillList)
        {
            if (skill.IsReady
                && owner.attackTarget.stats.SizeEllipse.IsCollidedWith(skill.CastEllipse))
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
        if (owner.forceReturn)
        {
            controller.ChangeState((int)CombatUnit.STATE.RETURN);
            return true;
        }

        if (!owner.HasTarget())
            controller.ChangeState((int)CombatUnit.STATE.IDLE);
        else
            controller.ChangeState((int)CombatUnit.STATE.TRACE);

        return true;
    }
}