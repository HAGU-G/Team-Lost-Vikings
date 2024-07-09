using UnityEngine;

public class UnitSpawnTester : MonoBehaviour
{

    private void Awake()
    {
        GameStarter.Instance.SetActiveOnComplete(gameObject);
        Debug.Log(SyncedTime.Now);
    }

}
