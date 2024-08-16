using UnityEngine;

public class DressListener : MonoBehaviour
{
    public event System.Action OnAttackHitEvent;
    public event System.Action OnSkillHitEvent;
    public event System.Action OnHitEffectEndEvent;

    public void ResetEvent()
    {
        OnAttackHitEvent = null;
        OnSkillHitEvent = null;
        OnHitEffectEndEvent = null;
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
}