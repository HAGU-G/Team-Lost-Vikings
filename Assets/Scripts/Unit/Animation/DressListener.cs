using UnityEngine;

public class DressListener : MonoBehaviour
{
    public event System.Action OnAttackHitEvent;
    public event System.Action OnSkillHitEvent;
    public event System.Action OnHitEffectEndEvent;
    public event System.Action OnGachaEndEventOnce;
    public event System.Action OnGachaShowEventOnce;
    public event System.Action OnGachaWaitEventOnce;

    public void ResetEvent()
    {
        OnAttackHitEvent = null;
        OnSkillHitEvent = null;
        OnHitEffectEndEvent = null;
        OnGachaEndEventOnce = null;
    }

    public void OnAttackHit()
    {
        OnAttackHitEvent?.Invoke();
    }
    
    public void OnSkillHit()
    {
        OnSkillHitEvent?.Invoke();
    }

    public void OnHitEnd()
    {
        OnHitEffectEndEvent?.Invoke();
    }

    public void OnGachaEnd()
    {
        OnGachaEndEventOnce?.Invoke();
        OnGachaEndEventOnce = null;
    }

    public void OnGachaShow()
    {
        OnGachaShowEventOnce?.Invoke();
        OnGachaShowEventOnce = null;
    }

    public void OnGachaWait()
    {

        OnGachaWaitEventOnce?.Invoke();
        OnGachaWaitEventOnce = null;
    }
}