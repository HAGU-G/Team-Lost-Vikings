using UnityEngine;

public class DressAnimator
{
    private static readonly int triggerIdle = Animator.StringToHash("Idle");
    private static readonly int triggerRun = Animator.StringToHash("Run");
    private static readonly int triggerAttack = Animator.StringToHash("Attack");

    private static readonly int paramJob = Animator.StringToHash("Job");
    private static readonly int paramMoveSpeed = Animator.StringToHash("MoveSpeed");
    private static readonly int paramAttackSpeed = Animator.StringToHash("AttackSpeed");

    private Animator animator;
    public DressListener listener;
    public StatFloat moveSpeed;
    public StatFloat attackSpeed;

    public void Init(Animator animator, UNIT_JOB job, StatFloat moveSpeed, StatFloat attackSpeed)
    {
        this.animator = animator;
        this.moveSpeed = moveSpeed;
        this.attackSpeed = attackSpeed;

        animator.SetInteger(paramJob, (int)job);

        listener = animator.GetComponent<DressListener>();
        if (listener == null)
            listener = animator.gameObject.AddComponent<DressListener>();
        listener.ResetEvent();
    }

    public void AnimIdle()
    {
        animator.ResetTrigger(triggerIdle);
        animator.SetTrigger(triggerIdle);
    }

    public void AnimRun()
    {
        animator.SetFloat(paramMoveSpeed, moveSpeed.Current);
        animator.ResetTrigger(triggerRun);
        animator.SetTrigger(triggerRun);
    }

    public void AnimAttack()
    {
        animator.SetFloat(paramAttackSpeed, 1f / attackSpeed.Current);
        animator.ResetTrigger(triggerAttack);
        animator.SetTrigger(triggerAttack);
    }


}