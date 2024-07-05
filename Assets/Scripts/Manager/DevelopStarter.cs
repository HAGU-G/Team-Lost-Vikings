using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DevelopSceneStarter : MonoBehaviour
{
    public List<AssetReference> scenes = new();
    private List<AsyncOperationHandle> operations = new();

    private int completeCount;
    private float progress;
    public TextMeshProUGUI progressText;

    private void Awake()
    {
        foreach (var scene in scenes)
        {
            operations.Add(Addressables.LoadSceneAsync(scene, LoadSceneMode.Additive));
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
        progressText.text = (progress * 50f).ToString();

        if (completeCount == operations.Count)
            SceneManager.UnloadSceneAsync(0);
    }
}
