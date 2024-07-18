using UnityEngine;

public class ObjectStarter : MonoBehaviour
{
    private void Awake()
    {
        if (GameStarter.Instance != null)
            GameStarter.Instance.SetActiveOnComplete(gameObject);
        else
            Debug.LogWarning("Starter가 아닌 씬에서 시작됨. 오작동할 수 있음.", gameObject);
        Destroy(this);
    }

}