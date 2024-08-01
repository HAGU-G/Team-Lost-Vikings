using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

public enum LOCATION
{
    NONE,
    VILLAGE,
    HUNTZONE
}

[Serializable]
public abstract class Unit : MonoBehaviour
{
    public UnitStats stats = null;
    public GameObject dress = null;

    public UnitSkills skills;
    public GameObject skillEffect;

    public DressAnimator animator = new();
    private Vector3 prePos;
    public bool isActing;


    public void SetPosition(Vector3 pos, bool playAnimation = false)
    {
        if (playAnimation)
        {
            LookAt(pos);
            animator.AnimRun();
        }
        transform.position = pos;
        stats.UpdateEllipsePosition();
    }

    public void Move(Vector3 destination, float deltaTime)
    {
        LookAt(destination);
        animator.AnimRun();
        if (stats == null)
            return;

        var direc = (destination - transform.position).normalized;
        transform.position += direc * stats.MoveSpeed.Current * deltaTime;
        stats.UpdateEllipsePosition();
    }

    public void Move(Transform target, float deltaTime) => Move(target.transform.position, deltaTime);


    public void LookAt(Vector3 target)
    {
        if (dress == null || target == null)
            return;

        var direc = target - transform.position;
        dress.transform.localScale = new Vector3(
            Mathf.Abs(dress.transform.localScale.x) * (direc.x > 0f ? -1f : 1f),
            dress.transform.localScale.y,
            dress.transform.localScale.z);

    }

    public void LookAt(Transform target) => LookAt(target.position);

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
