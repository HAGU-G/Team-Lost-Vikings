using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Color = UnityEngine.Color;

public class DressAnimator
{
    private static readonly string nameIdle = "Idle";
    private static readonly string nameRun = "Run";

    private static readonly int triggerIdle = Animator.StringToHash(nameIdle);
    private static readonly int triggerRun = Animator.StringToHash(nameRun);
    private static readonly int triggerAttack = Animator.StringToHash("Attack");
    private static readonly int triggerSkill = Animator.StringToHash("Skill");
    private static readonly int triggerDeath = Animator.StringToHash("Death");
    private static readonly int triggerHit = Animator.StringToHash("Hit");
    private static readonly int triggerGacha = Animator.StringToHash("Gacha");

    private static readonly int paramMoveSpeed = Animator.StringToHash("MoveSpeed");
    private static readonly int paramAttackSpeed = Animator.StringToHash("AttackSpeed");
    private static readonly int paramAttackMotion = Animator.StringToHash("AttackMotion");

    private Animator animator;
    public DressListener listener;
    public StatFloat moveSpeed;
    public StatFloat attackSpeed;
    public float castTime;
    private bool isHide;

    private List<SpriteRenderer> renderers = new();
    private List<Color> defaultColors = new();
    private List<Color> prevColors = new();

    public void Init(Animator animator, StatFloat moveSpeed, StatFloat attackSpeed)
    {
        this.animator = animator;
        this.moveSpeed = moveSpeed;
        this.attackSpeed = attackSpeed;

        listener = animator.GetComponent<DressListener>();
        if (listener == null)
            listener = animator.gameObject.AddComponent<DressListener>();
        listener.ResetEvent();

        renderers = animator.GetComponentsInChildren<SpriteRenderer>().ToList();
        defaultColors.Clear();
        prevColors.Clear();
        foreach (var renderer in renderers)
        {
            if(renderer == null)
                continue;

            defaultColors.Add(renderer.color);
            prevColors.Add(renderer.color);
        }
        listener.OnHitEffectEndEvent += ResetColor;
    }

    public void AnimDeath()
    {
        if (animator == null)
            return;

        animator.SetTrigger(triggerDeath);
    }

    public void AnimIdle()
    {
        if (animator == null
            || animator.GetCurrentAnimatorStateInfo(0).IsName(nameIdle))
            return;

        animator.SetTrigger(triggerIdle);
    }

    public void AnimRun()
    {
        if (animator == null
            || animator.GetCurrentAnimatorStateInfo(0).IsName(nameRun))
            return;

        animator.SetFloat(paramMoveSpeed, moveSpeed.Current);
        animator.SetTrigger(triggerRun);
    }

    public void AnimAttack(ATTACK_MOTION motion)
    {
        //TODO 프리펩 미리 로드 후 생성시 바로 작동할 수 있도록 변경 필요
        if (animator == null)
            return;

        animator.SetInteger(paramAttackMotion, (int)motion);
        var clipNum = motion switch
        {
            ATTACK_MOTION.MAGIC => 5,
            ATTACK_MOTION.BOW => 4,
            _ => 6
        };
        float multiplier = animator.runtimeAnimatorController.animationClips[clipNum].length / attackSpeed.Current;
        animator.SetFloat(paramAttackSpeed, multiplier);
        animator.SetTrigger(triggerAttack);
    }

    public void AnimSkill(ATTACK_MOTION motion, float castTime)
    {
        if (animator == null)
            return;

        animator.SetInteger(paramAttackMotion, (int)motion);
        var clipNum = motion switch
        {
            ATTACK_MOTION.MAGIC => 8,
            ATTACK_MOTION.BOW => 7,
            _ => 9
        };
        float multiplier = animator.runtimeAnimatorController.animationClips[clipNum].length / castTime;
        animator.SetFloat(paramAttackSpeed, multiplier);
        animator.SetTrigger(triggerSkill);
    }

    public void AnimHit()
    {
        if (animator == null)
            return;

        animator.SetTrigger(triggerHit);
        var hitColor = GameSetting.Instance.hitEffectColor;
        SetColor(hitColor);
    }

    public void AnimGacha()
    {
        if (animator == null)
            return;

        animator.SetTrigger(triggerGacha);
    }

    public void ResetColor()
    {
        if (isHide)
        {
            for (int i = 0; i < prevColors.Count; i++)
            {
                prevColors[i] = defaultColors[i];
            }
        }
        else
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i] == null)
                    continue;

                renderers[i].color = defaultColors[i];
            }
        }
    }

    public void SetAlpha(float alpha)
    {
        if (isHide)
        {
            for (int i = 0; i < prevColors.Count; i++)
            {
                if (renderers[i] == null)
                    continue;

                var color = renderers[i].color;
                color.a = alpha;
                prevColors[i] = color;
            }
        }
        else
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i] == null)
                    continue;

                var color = renderers[i].color;
                color.a = alpha;
                renderers[i].color = color;
            }
        }
    }

    public void SetColor(Color color)
    {
        if (isHide)
        {
            for (int i = 0; i < prevColors.Count; i++)
            {
                prevColors[i] = color;
            }
        }
        else
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i] == null)
                    continue;

                prevColors[i] = renderers[i].color;
                renderers[i].color = color;
            }
        }
    }

    public void SetHide(bool isHide)
    {
        if (this.isHide == isHide)
            return;

        this.isHide = isHide;
        if (isHide)
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i] == null)
                    continue;

                prevColors[i] = renderers[i].color;
                renderers[i].color = Color.clear;
            }
        }
        else
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i] == null)
                    continue;

                renderers[i].color = prevColors[i];
            }
        }
    }
}