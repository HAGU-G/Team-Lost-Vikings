using UnityEngine;

public class UseSkillOnHunt : State<CombatUnit>
{
    private bool isUsing = false;
    private bool isPlaying = false;
    private int useCount = 0;
    private float timer = 0f;
    private Skill skill = null;
    private CombatUnit target;
    private Vector3 lastTargetPos;

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
        lastTargetPos = target.transform.position;

        if (owner.animator.listener != null)
            owner.animator.listener.OnSkillHitEvent += UseSkill;

        skill.ResetActiveValue();
        owner.LookAt(owner.attackTarget.transform);
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
            owner.LookAt(target.transform);
            lastTargetPos = target.transform.position;
        }
        else
        {
            target = null;
        }
        isPlaying = true;

        if (skill.Data.SkillCastTime <= 0f)
            UseSkill();
        else
            owner.animator.AnimSkill(skill.Data.SkillAnime, skill.Data.SkillCastTime);

        owner.isTargetFixed = true;
        timer = 0f;
    }

    private void UseSkill()
    {
        useCount++;
        isPlaying = false;

        if (owner.IsDead)
            return;

        owner.LookAt(lastTargetPos);
        SoundManager.PlaySFX(skill.Data.SkillSEName, owner.stats.Location, owner.CurrentHuntZone.HuntZoneNum);
        if (owner.HasTarget())
            skill?.Use(target);
        else
            skill?.Use(lastTargetPos);
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