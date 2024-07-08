using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Text;

public class GameStarter : MonoBehaviour
{
    private GameStarter() { }

    public static GameStarter Instance { get; private set; }

    public List<AssetReference> scenes = new();
    private AsyncOperationHandle operation = default;

    private int completeCount = 0;
    private float completedProgress;

    public event System.Action OnCompleted;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        OnCompleted += () => 
        {
            gameObject.SetActive(false); 
        };
        LoadScenes();
    }

    private void LoadScenes()
    {
        completedProgress = (float)completeCount / scenes.Count;

        operation = Addressables.LoadSceneAsync(scenes[completeCount], LoadSceneMode.Additive);
        operation.Completed += OnLoadCompleted;
    }

    private void OnLoadCompleted(AsyncOperationHandle operationHandle)
    {
        if (operationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            completeCount++;

            if (completeCount == scenes.Count)
            {
                UpdateProgress();
                OnCompleted?.Invoke();
                OnCompleted = null;
            }
            else
            {
                LoadScenes();
            }
        }
        else
        {
            OnLoadFailed();
        }

    }

    private void OnLoadFailed()
    {
        var sb = new StringBuilder();
        sb.AppendLine("씬 로드 실패");

        if (completeCount == 0)
        {
            Debug.LogError(sb);
            return;
        }

        sb.Append("완료 : ");
        for (int i = 0; i < completeCount; i++)
        {
            sb.Append($"{i}, ");
        }

        sb.AppendLine("\n실패 : ");
        for (int i = completeCount; i < scenes.Count; i++)
        {
            sb.Append($"{i}, ");
        }

        Debug.LogError(sb);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    private void UpdateProgress()
    {
        Debug.Log($"{(completedProgress + operation.PercentComplete / scenes.Count) * 100f}%");
    }

#if UNITY_EDITOR
    private void Update()
    {
        UpdateProgress();
    }
#endif

    /// <summary>
    /// 게임오브젝트 비활성화 후
    /// 씬이 모두 로드 됐을 때 활성화
    /// </summary>
    /// <param name="gameobject">대상</param>
    public void SetActiveOnComplete(GameObject gameObject)
    {
        gameObject.SetActive(false);
        OnCompleted += () => 
        { 
            gameObject.SetActive(true); 
        };

    }

}