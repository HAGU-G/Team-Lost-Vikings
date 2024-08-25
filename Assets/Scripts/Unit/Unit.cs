using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public abstract class Unit : MonoBehaviour, IPointerClickHandler
{
    public HPBar hpBar;
    public UnitStats stats = null;
    [HideInInspector] public GameObject dress = null;
    public DressAnimator animator = new();
    [HideInInspector] public bool isActing = false;
    [HideInInspector] public bool isFlip = true;

    private SortingGroup sortingGroup = null;
    public Vector3 LastDirection { get; private set; }
    public bool IsMoved { get; private set; } = false;

    public bool IsDead
    {
        get
        {
            if (stats == null)
                return true;
            else
                return stats.isDead;
        }
        protected set
        {
            if (stats != null)
                stats.isDead = value;
        }
    }

    public event System.Action OnUpdated;
    public event System.Action OnRemoveOnce;

    public virtual void Init()
    {
        if (!TryGetComponent(out sortingGroup))
            sortingGroup = gameObject.AddComponent<SortingGroup>();
        sortingGroup.sortAtRoot = true;
    }

    public virtual void OnEnable()
    {

    }

    public virtual void ResetUnit(UnitStats stats)
    {
        ResetEvents();

        if (stats == null)
            Debug.LogWarning("유닛의 스탯이 재설정되지 않았습니다.", gameObject);
        else
            this.stats = stats;

        isActing = false;
        if (stats.Data.UnitType == UNIT_TYPE.MONSTER)
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
    }

    protected virtual void Update()
    {
        if (animator != null)
            animator.SetHide(GameManager.cameraManager.isHideUnits);

        stats.UpdateTimers(Time.deltaTime);
        OnUpdated?.Invoke();
    }

    protected virtual void LateUpdate()
    {
        IsMoved = false;

        var sortingOrder = Mathf.FloorToInt(-transform.position.y);
        sortingGroup.sortingOrder = sortingOrder;
        hpBar.canvas.sortingOrder = sortingOrder;

        if (stats != null
            && !GameManager.IsPlayingAnimation
            && GameManager.cameraManager != null
            && !GameManager.cameraManager.isFocusOnUnit)
        {
            hpBar.gameObject.SetActive(true);
            hpBar.slider.value = stats.HP.Ratio;
        }
        else
        {
            hpBar.gameObject.SetActive(false);
        }
    }

    public virtual void OnRelease()
    {
        if (stats.Data.UnitType == UNIT_TYPE.CHARACTER)
        {
            stats = null;
        }
    }

    public virtual void RemoveUnit()
    {
        OnRemoveOnce?.Invoke();
        OnRemoveOnce = null;
    }

    protected virtual void ResetEvents()
    {
        OnUpdated = null;
        OnRemoveOnce = null;
    }

    protected virtual void OnAnimationAttackHit() { }

    public void SetPosition(Vector3 pos, bool playAnimation = false)
    {
        IsMoved = true;
        LastDirection = pos - transform.position;
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
        IsMoved = true;
        LastDirection = direction;
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

        isFlip = direction.x <= 0f;

        dress.transform.localScale = new Vector3(
            Mathf.Abs(dress.transform.localScale.x) * (isFlip ? 1f : -1f),
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (stats.Data.UnitType != UNIT_TYPE.CHARACTER)
            return;

        var unitOnVillage = GetComponent<UnitOnVillage>();
        if (unitOnVillage != null
            && unitOnVillage.currentState == UnitOnVillage.STATE.INTERACT)
        {
            return;
        }

        GameManager.soundManager.PlayUnitSFX();
        var cm = GameManager.cameraManager;
        cm.StartFocusOnUnit(stats);
        GameManager.uiManager.currentUnitStats = stats;

        if (GameManager.villageManager.constructMode.isConstructMode)
        {
            var constructMode = GameManager.uiManager.windows[WINDOW_NAME.CONSTRUCT_MODE] as UIConstructMode;
            constructMode.FinishConstructMode();
        }

        GameManager.uiManager.windows[WINDOW_NAME.TOUCH_UNIT_BUTTONS].Open();
    }

}