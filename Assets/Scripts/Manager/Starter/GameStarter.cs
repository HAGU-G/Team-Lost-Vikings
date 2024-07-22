using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Text;
using UnityEditorInternal;

public class GameStarter : MonoBehaviour
{
    private GameStarter() { }

    public static GameStarter Instance { get; private set; }

    public List<AssetReference> scenes = new();
    private AsyncOperationHandle operation = default;

    private int completeCount = 0;
    private float completedProgress;
    public bool IsSceneLoaded { get; private set; } = false;

    public event System.Action OnCompleted;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DataTableManager.Load();
        LoadScenes();
    }

    private void LoadScenes()
    {
        completedProgress = (float)completeCount / scenes.Count;

        operation = Addressables.LoadSceneAsync(scenes[completeCount], LoadSceneMode.Additive);
        operation.Completed += OnSceneLoadCompleted;
    }

    private void OnSceneLoadCompleted(AsyncOperationHandle operationHandle)
    {
        if (operationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            completeCount++;

            if (completeCount == scenes.Count)
                IsSceneLoaded = true;
            else
                LoadScenes();

        }
        else
        {
            OnSceneLoadFailed();
        }

    }

    private void OnSceneLoadFailed()
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
        DataTableManager.Update();
        float current = DataTableManager.progress + completedProgress + operation.PercentComplete / scenes.Count;
        Debug.Log($"{current / 2f * 100f}%");
    }

#if UNITY_EDITOR
    private void Update()
    {
        UpdateProgress();

        if (IsSceneLoaded
            && DataTableManager.IsReady)
        {
            UpdateProgress();
            GameManager.OnGameStart();
            OnCompleted?.Invoke();
            OnCompleted = null;
            gameObject.SetActive(false);
        }
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