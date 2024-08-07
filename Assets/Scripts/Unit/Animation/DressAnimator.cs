using UnityEngine;

public class DressAnimator
{
    private static readonly string nameIdle = "Idle";
    private static readonly string nameRun = "Run";

    private static readonly int triggerIdle = Animator.StringToHash(nameIdle);
    private static readonly int triggerRun = Animator.StringToHash(nameRun);
    private static readonly int triggerAttack = Animator.StringToHash("Attack");
    private static readonly int triggerSkill = Animator.StringToHash("Skill");

    private static readonly int paramMoveSpeed = Animator.StringToHash("MoveSpeed");
    private static readonly int paramAttackSpeed = Animator.StringToHash("AttackSpeed");
    private static readonly int paramAttackMotion = Animator.StringToHash("AttackMotion");

    private Animator animator;
    public DressListener listener;
    public StatFloat moveSpeed;
    public StatFloat attackSpeed;
    public float castTime;

    public void Init(Animator animator, StatFloat moveSpeed, StatFloat attackSpeed)
    {
        this.animator = animator;
        this.moveSpeed = moveSpeed;
        this.attackSpeed = attackSpeed;

        listener = animator.GetComponent<DressListener>();
        if (listener == null)
            listener = animator.gameObject.AddComponent<DressListener>();
        listener.ResetEvent();
    }

    public void AnimIdle()
    {
        if (animator == null
            || animator.GetCurrentAnimatorStateInfo(0).IsName(nameIdle))
            return;

        animator.ResetTrigger(triggerIdle);
        animator.SetTrigger(triggerIdle);
    }

    public void AnimRun()
    {
        if (animator == null
            || animator.GetCurrentAnimatorStateInfo(0).IsName(nameRun))
            return;

        animator.SetFloat(paramMoveSpeed, moveSpeed.Current);
        animator.ResetTrigger(triggerRun);
        animator.SetTrigger(triggerRun);
    }

    public void AnimAttack(ATTACK_MOTION motion)
    {
        //TODO 프리펩 미리 로드 후 생성시 바로 작동할 수 있도록 변경 필요
        if (animator == null)
            return;

        animator.SetInteger(paramAttackMotion, (int)motion);
        animator.SetFloat(paramAttackSpeed, 1f / attackSpeed.Current);
        animator.ResetTrigger(triggerAttack);
        animator.SetTrigger(triggerAttack);
    }

    public void AnimSkill(ATTACK_MOTION motion, float castTime)
    {
        if (animator == null)
            return;

        animator.SetInteger(paramAttackMotion, (int)motion);
        animator.SetFloat(paramAttackSpeed, 1f / castTime);
        animator.ResetTrigger(triggerSkill);
        animator.SetTrigger(triggerSkill);
    }
}