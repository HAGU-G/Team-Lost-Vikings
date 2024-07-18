using UnityEngine;

public class MonsterRegenPoint : MonoBehaviour, IObserver<Monster>
{
    #region INSPECTOR
    public SpriteRenderer spriteRenderer;
    #endregion

    public int maxSpawnCount = 1;
    private int spawnCount;
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
        IsReady = ++spawnCount < maxSpawnCount;
        monster.Subscribe(this);
    }

    public void UpdateIndicator()
    {
        if (_isReady)
            spriteRenderer.color = Color.green;
        else
            spriteRenderer.color = Color.red;
    }

    public void ReceiveNotification(Monster subject, NOTIFY_TYPE type = NOTIFY_TYPE.NONE)
    {
        if (type == NOTIFY_TYPE.DEAD || type == NOTIFY_TYPE.REMOVE)
            IsReady = --spawnCount < maxSpawnCount;
    }
}