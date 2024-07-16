using UnityEngine;

public class ObjectStarter : MonoBehaviour
{
    private void Awake()
    {
        GameStarter.Instance.SetActiveOnComplete(gameObject);
        Destroy(this);
    }

}