using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;

public abstract class Unit : MonoBehaviour
{
    public UnitStats stats = null;

    public GameObject dress = null;
    public DressAnimator animator = new();
    public bool isActing = false;

    public UnitSkills skills;
    public GameObject skillEffect;
    private SortingGroup sortingGroup = null;

    public bool IsDead { get; protected set; }

    public virtual void Init() 
    {
        if(!TryGetComponent(out sortingGroup))
            sortingGroup = gameObject.AddComponent<SortingGroup>();
        sortingGroup.sortAtRoot = true;
    }
    public virtual void ResetUnit(UnitStats stats)
    {
        if (stats == null)
            Debug.LogWarning("유닛의 스탯이 재설정되지 않았습니다.", gameObject);
        else
            this.stats = stats;

        isActing = false;
        IsDead = false;

        if (dress != null)
            Addressables.ReleaseInstance(dress);

        Addressables.InstantiateAsync(this.stats.Data.UnitAssetFileName, transform)
            .Completed += (handle) =>
            {
                if (dress != null)
                    Destroy(dress);

                dress = handle.Result;
                animator.Init(
                    handle.Result.GetComponentInChildren<Animator>(),
                    this.stats.MoveSpeed,
                    this.stats.AttackSpeed);

                animator.listener.OnAttackHitEvent += OnAnimationAttackHit;
            };

        this.stats.ResetEllipse(transform);
        ResetEvents();
    }

    protected virtual void Update()
    {
        sortingGroup.sortingOrder = Mathf.FloorToInt(-transform.position.y);
    }


    public virtual void OnRelease()
    {
        if (stats.Data.UnitType == UNIT_TYPE.CHARACTER)
        {
            stats = null;
        }
    }

    public virtual void RemoveUnit() { }

    protected virtual void ResetEvents() { }

    protected virtual void OnAnimationAttackHit() { }

    public void SetPosition(Vector3 pos, bool playAnimation = false)
    {
        if (playAnimation && animator != null)
        {
            LookAt(pos);
            animator.AnimRun();
        }
        transform.position = pos;
        stats.UpdateEllipsePosition();
    }




    //Move, Look

    public void MoveToDestination(Transform target, float deltaTime) => MoveToDestination(target.transform.position, deltaTime);
    public void MoveToDestination(Vector3 destination, float deltaTime) => MoveToDirection(destination - transform.position, deltaTime);
    public void MoveToDirection(Vector3 direction, float deltaTime)
    {
        LookDirection(direction);
        animator.AnimRun();

        if (stats == null)
            return;

        transform.position += direction.normalized * stats.MoveSpeed.Current * deltaTime;
        stats.UpdateEllipsePosition();
    }

    public void LookDirection(Vector3 direction)
    {
        if (dress == null)
            return;

        dress.transform.localScale = new Vector3(
            Mathf.Abs(dress.transform.localScale.x) * (direction.x > 0f ? -1f : 1f),
            dress.transform.localScale.y,
            dress.transform.localScale.z);
    }

    public void LookAt(Vector3 target) => LookDirection(target - transform.position);
    public void LookAt(Transform target)
    {
        if (target == null)
            return;

        LookAt(target.position);
    }

}