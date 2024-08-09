using UnityEditorInternal;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class UseSkillOnHunt : State<CombatUnit>
{
    private bool isUsing = false;
    private bool isPlaying = false;
    private int useCount = 0;
    private float timer = 0f;
    private Skill skill = null;
    private Vector3 targetPos;
    private CombatUnit target;

    public override void EnterState()
    {
        owner.currentState = CombatUnit.STATE.SKILL;
        //owner.spriteRenderer.color = Color.magenta;
        owner.isActing = true;

        skill = owner.stats.Skills[owner.usingSkillNum];
        isUsing = true;
        useCount = 0;
        timer = skill.Data.SkillCastTime;
        target = owner.attackTarget;
        targetPos = owner.attackTarget.transform.position;

        if (owner.animator.listener != null)
            owner.animator.listener.OnSkillHitEvent += UseSkill;

        skill.ResetActiveValue();
        owner.LookAt(owner.attackTarget.transform);

        Debug.Log($"스킬 시전 {skill.Data.SkillName}");
    }

    public override void ExitState()
    {
        owner.isActing = false;
        isPlaying = false;
        owner.usingSkillNum = -1;

        if (owner.animator.listener != null)
            owner.animator.listener.OnSkillHitEvent -= UseSkill;
    }

    public override void ResetState()
    {
        owner.isActing = false;
        isPlaying = false;
        owner.usingSkillNum = -1;

        if (owner.animator.listener != null)
            owner.animator.listener.OnSkillHitEvent -= UseSkill;
    }

    public override void Update()
    {
        timer += Time.deltaTime;

        if (Transition() || timer < skill.Data.SkillCastTime || isPlaying)
            return;

        if (useCount >= skill.Data.SkillActiveNum)
        {
            isUsing = false;
            return;
        }

        if (target != null && !target.IsDead && target.gameObject.activeSelf)
        { 
            targetPos = target.transform.position;
            owner.LookAt(target.transform);
        }
        else
        {
            target = null;
        }

        isPlaying = true;
        owner.animator.AnimSkill(skill.Data.SkillAnime, skill.Data.SkillCastTime);

        owner.isTargetFixed = true;
        timer = 0f;
    }

    private void UseSkill()
    {
        skill?.Use(targetPos);
        useCount++;
        isPlaying = false;
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