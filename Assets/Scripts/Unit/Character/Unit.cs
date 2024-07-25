using System;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public abstract class Unit : MonoBehaviour, IStatUsable
{
    public UnitStats stats = null;
    public GameObject dress = null;

    public virtual STAT_GROUP StatGroup => STAT_GROUP.UNIT_ON_VILLAGE;
    public Stats GetStats => stats;
    public UnitSkills skills;
    public GameObject skillEffect;

    public DressAnimator animator = new();
    private Vector3 prePos;
    public bool isActing;

    public void LookTarget(Transform target)
    {
        if(dress == null || target == null)
            return;

        var direc = target.position - transform.position;
        dress.transform.localScale = new Vector3(
            Mathf.Abs(dress.transform.localScale.x) * (direc.x > 0f ? -1f : 1f),
            dress.transform.localScale.y,
            dress.transform.localScale.z);
    }    

    public void UpdateAnimator()
    {
        if (!isActing && animator != null && dress != null)
        {
            if (transform.position != prePos)
            {
                float preLook = Mathf.Sign(dress.transform.localScale.x);
                float currLook = Mathf.Sign((transform.position - prePos).x) * -1f;
                bool flip = (preLook != currLook) && currLook != 0f;

                dress.transform.localScale = new Vector3(
                    dress.transform.localScale.x * (flip ? -1f : 1f),
                    dress.transform.localScale.y,
                    dress.transform.localScale.z);

                animator.AnimRun();
            }
            else
            {
                animator.AnimIdle();
            }
        }
        prePos = transform.position;
    }

    /// <summary>
    /// 오브젝트 풀 OnCreate에서 호출
    /// </summary>
    public virtual void Init()
    {
    }

    /// <summary>
    /// 오브젝트 풀 OnGet에서 호출
    /// </summary>
    public virtual void ResetUnit(UnitStats unitStats)
    {
        if (unitStats == null)
            Debug.LogWarning("유닛의 스탯이 재설정되지 않았습니다.", gameObject);
        else
            stats = unitStats;

        if (dress != null)
            Addressables.ReleaseInstance(dress);

        Addressables.InstantiateAsync(stats.AssetFileName, transform)
        .Completed += (handle) =>
        {
            if (dress != null)
                Destroy(dress);

            dress = handle.Result;
            animator.Init(
                handle.Result.GetComponentInChildren<Animator>(),
                stats.Job,
                stats.MoveSpeed,
                stats.AttackSpeed);

            animator.listener.onAttackHit += OnAnimationAttackHit;
        };

        isActing = false;
        stats?.ResetEllipse(transform);
        ResetEvents();
    }
    protected virtual void OnAnimationAttackHit() { }
    /// <summary>
    /// 오브젝트 풀 OnRelease에서 호출
    /// </summary>
    public virtual void OnRelease()
    {
        //stats.objectTransform = null;
        stats = null;
    }

    public virtual void RemoveUnit()
    {
    }

    protected virtual void ResetEvents() { }
}
