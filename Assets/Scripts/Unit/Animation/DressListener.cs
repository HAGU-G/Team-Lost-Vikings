using UnityEngine;

public class DressListener : MonoBehaviour
{
    public event System.Action OnAttackHitEvent;
    public void OnAttackHit()
    {
        OnAttackHitEvent?.Invoke();
    }
    public void ResetEvent()
    {
        OnAttackHitEvent = null;
    }
}