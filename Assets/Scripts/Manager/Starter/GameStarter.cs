using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Text;
using UnityEngine.UI;
using TMPro;

public class GameStarter : MonoBehaviour
{
    private GameStarter() { }

    public static GameStarter Instance { get; private set; }


    public GameObject loadingUI;
    public GameObject netWorkErrorUI;
    public Slider loadingSlider;
    public TextMeshProUGUI loadingText;

    public List<AssetReference> scenes = new();
    private AsyncOperationHandle operation = default;

    private int completeCount = 0;
    private float completedProgress;
    public bool IsSceneLoaded { get; private set; } = false;

    private float syncTimer = 0f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;

        SyncedTime.Sync();
        DataTableManager.Load();
        LoadScenes();

        GameManager.Subscribe(EVENT_TYPE.GAME_READY, LoadingUIOff);
    }

    private void LoadingUIOff()
    {
        loadingUI.SetActive(false);
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
        var currentLoad = DataTableManager.progress + completedProgress + operation.PercentComplete / scenes.Count;
        var loadNormalize = currentLoad / 2f;
        if (loadingSlider != null)
            loadingSlider.value = loadNormalize;
        if (loadingText != null)
            loadingText.text = $"{loadNormalize * (SyncedTime.IsSynced ? 100f : 99f):0}%";
    }

    private void Update()
    {
        syncTimer += Time.unscaledDeltaTime;
        if (!SyncedTime.IsSynced && syncTimer >= GameSetting.Instance.firstSyncTimeout)
        {
            netWorkErrorUI.gameObject.SetActive(true);
            return;
        }

        UpdateProgress();

        if (SyncedTime.IsSynced
            && IsSceneLoaded
            && DataTableManager.IsReady)
        {
            UpdateProgress();
            GameManager.GameLoaded();
            gameObject.SetActive(false);
        }
    }

    public void OnButtonGameQuit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}