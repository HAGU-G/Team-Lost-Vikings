using UnityEngine;

public class DressListener : MonoBehaviour
{
    public event System.Action onAttackHit;
    public void OnAttackHit()
    {
        onAttackHit?.Invoke();
    }
    public void ResetEvent()
    {
        onAttackHit = null;
    }
}