public class UseSkillOnHunt : State<UnitOnHunt>
{
    private float attackTimer;

    public override void EnterState()
    {
        owner.currentState = UnitOnHunt.STATE.ATTACK;
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
        if (owner.attackTarget == null)
            controller.ChangeState((int)UnitOnHunt.STATE.IDLE);
        else
            controller.ChangeState((int)UnitOnHunt.STATE.TRACE);

        return true;
    }
}