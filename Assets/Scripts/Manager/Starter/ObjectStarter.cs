using UnityEngine;

public class ObjectStarter : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Subscribe(EVENT_TYPE.LOADED, SetActiveOnComplete);
        gameObject.SetActive(false);
    }

    private void SetActiveOnComplete()
    {
        gameObject.SetActive(true);
        enabled = false;
    }
}