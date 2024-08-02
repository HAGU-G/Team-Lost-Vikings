using UnityEngine;
using UnityEngine.AddressableAssets;
using static UnityEngine.GraphicsBuffer;

public abstract class Unit : MonoBehaviour
{
    public abstract Stats GetStats { get; }

    public GameObject dress = null;
    public DressAnimator animator = new();
    public bool isActing = false;

    public UnitSkills skills;
    public GameObject skillEffect;

    public bool IsDead { get; protected set; }

    public virtual void Init() { }
    protected void ResetBase()
    {
        isActing = false;
        IsDead = false;

        if (dress != null)
            Addressables.ReleaseInstance(dress);

        Addressables.InstantiateAsync(GetStats.Data.UnitAssetFileName, transform)
            .Completed += (handle) =>
            {
                if (dress != null)
                    Destroy(dress);

                dress = handle.Result;
                animator.Init(
                    handle.Result.GetComponentInChildren<Animator>(),
                    GetStats.MoveSpeed,
                    GetStats.AttackSpeed);

                animator.listener.OnAttackHitEvent += OnAnimationAttackHit;
            };

        GetStats.ResetEllipse(transform);
        ResetEvents();
    }

    public virtual void OnRelease() { }

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
        GetStats.UpdateEllipsePosition();
    }




    //Move, Look

    public void MoveToDestination(Transform target, float deltaTime) => MoveToDestination(target.transform.position, deltaTime);
    public void MoveToDestination(Vector3 destination, float deltaTime) => MoveToDirection(destination - transform.position, deltaTime);
    public void MoveToDirection(Vector3 direction, float deltaTime)
    {
        LookDirection(direction);
        animator.AnimRun();

        if (GetStats == null)
            return;

        transform.position += direction.normalized * GetStats.MoveSpeed.Current * deltaTime;
        GetStats.UpdateEllipsePosition();
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
        if(target == null)
            return;

        LookAt(target.position); 
    }

}