using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DevelopStarter : MonoBehaviour
{
    public static DevelopStarter Instance { get; private set; }

    public List<AssetReference> scenes = new();
    private List<AsyncOperationHandle> operations = new();

    private int completeCount;
    private float progress;

    public event System.Action OnCompleted;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        foreach (var scene in scenes)
        {

            //TODO 하나 로딩 완료되면 다음거 로딩
            var operation = Addressables.LoadSceneAsync(scene, LoadSceneMode.Additive, false);
            operations.Add(operation);
        }
    }

    private void Update()
    {
        progress = 0f;
        foreach (var operation in operations)
        {
            progress += operation.PercentComplete;
            if (operation.IsDone)
                completeCount++;
        }
        Debug.Log($"{progress * 100f / operations.Count}%");

        if (completeCount > operations.Count)
        {
            OnCompleted?.Invoke();
            Destroy(gameObject);
        }
    }
}
