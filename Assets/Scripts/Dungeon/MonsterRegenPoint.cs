using UnityEngine;

public class MonsterRegenPoint : MonoBehaviour, IObserver<Monster>
{
    #region INSPECTOR
    public SpriteRenderer spriteRenderer;
    #endregion

    private bool _isReady = true;
    public bool IsReady
    {
        get => _isReady;
        set
        {
            _isReady = value;
            UpdateIndicator();
        }
    }

#if !UNITY_EDITOR
    private void Awake()
    {
        spriteRenderer.gameObject.SetActive(true);
    }
#endif

    public void Spawned(Monster monster)
    {
        IsReady = false;
        monster.Subscribe(this);
    }

    public void UpdateIndicator()
    {
#if UNITY_EDITOR
        if (_isReady)
            spriteRenderer.color = Color.green;
        else
            spriteRenderer.color = Color.red;
#endif
    }

    public void ReceiveNotification(Monster subject, NOTIFY_TYPE type = NOTIFY_TYPE.NONE)
    {
        if (type == NOTIFY_TYPE.DEAD)
            IsReady = true;
    }
}