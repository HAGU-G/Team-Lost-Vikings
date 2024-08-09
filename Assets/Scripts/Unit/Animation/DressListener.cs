using UnityEngine;

public class DressListener : MonoBehaviour
{
    public event System.Action OnAttackHitEvent;
    public event System.Action OnSkillHitEvent;

    public void ResetEvent()
    {
        OnAttackHitEvent = null;
        OnSkillHitEvent = null;
    }

    public void OnAttackHit()
    {
        OnAttackHitEvent?.Invoke();
    }
    
    public void OnSkillHit()
    {
        OnSkillHitEvent?.Invoke();
    }
}