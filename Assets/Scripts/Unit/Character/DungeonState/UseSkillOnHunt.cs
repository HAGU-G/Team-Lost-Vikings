using UnityEditorInternal;
using UnityEngine;

public class UseSkillOnHunt : State<CombatUnit>
{
    private bool isUsing = false;
    private int useCount = 0;
    private float timer = 0f;
    private Skill skill = null;
    private Vector3 targetPos;

    public override void EnterState()
    {
        owner.currentState = CombatUnit.STATE.SKILL;
        //owner.spriteRenderer.color = Color.magenta;
        owner.isActing = true;

        skill = owner.stats.Skills[owner.usingSkillNum];
        isUsing = true;
        useCount = 0;
        timer = skill.Data.ActiveTerm;

        skill.ResetActiveValue();
        targetPos = owner.attackTarget.transform.position;
    }

    public override void ExitState()
    {
        owner.isActing = false;
        owner.usingSkillNum = -1;
    }

    public override void ResetState()
    {
        owner.isActing = false;
        owner.usingSkillNum = -1;
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (Transition() || timer >= skill.Data.ActiveTerm)
            return;


        owner.animator?.AnimSkill(skill.Data.SkillAnime, skill.Data.CastTime);
        skill.Use();

        owner.isTargetFixed = true;
        timer = 0f;

        if (++useCount >= skill.Data.ActiveNum)
            isUsing = false;

    }

    protected override bool Transition()
    {
        if (isUsing)
            return false;

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