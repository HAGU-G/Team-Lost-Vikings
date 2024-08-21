//임시로 사용 중

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EVENT_TYPE
{
    LOADED,
    INIT,
    START,
    PAUSE,
    RESUME,
    QUIT,
    UPGRADE,
    CONSTRUCT,
    CONFIGURE,
    GAME_READY,
}

public static class GameManager
{
    public static VillageManager villageManager = null;
    public static HuntZoneManager huntZoneManager = null;
    public static UIManager uiManager = null;
    public static UnitManager unitManager = null;
    public static InputManager inputManager = null;
    public static PlayerManager playerManager = null;
    public static QuestManager questManager = null;
    public static ItemManager itemManager = null;
    public static CameraManager cameraManager = null;
    public static SoundManager soundManager = null;
    public static DialogManager dialogManager = new();
    public static EffectManager effectManager = null;

    public static bool IsReady { get; set; } = false;

    private static IDictionary<EVENT_TYPE, UnityEvent> events = new Dictionary<EVENT_TYPE, UnityEvent>();

    private static float prevTimeScale;

    public static void Subscribe(EVENT_TYPE eventType, in UnityAction listener)
    {
        UnityEvent action = null;

        if (events.TryGetValue(eventType, out action))
        {
            action.RemoveListener(listener);
            action.AddListener(listener);
        }
        else
        {
            action = new UnityEvent();
            action.AddListener(listener);
            events.Add(eventType, action);
        }
    }

    public static void UnSubscribe(EVENT_TYPE eventType, in UnityAction listener)
    {
        if (events.TryGetValue(eventType, out var action))
        {
            Debug.LogWarning("해제됨");
            action.RemoveListener(listener);
        }
    }

    public static void Publish(EVENT_TYPE eventType)
    {
        if (events.TryGetValue(eventType, out var action))
        {
            action.Invoke();
        }
    }


    //TESTCODE
    public static void GameLoaded()
    {
        UnitStats.existIDs.Clear();

        Debug.Log(EVENT_TYPE.LOADED);
        Publish(EVENT_TYPE.LOADED);
        Debug.Log(EVENT_TYPE.INIT);
        Publish(EVENT_TYPE.INIT);

        Debug.Log("LOAD_SAVE_DATA");
        SaveManager.LoadGame();

        playerManager ??= new();
        itemManager ??= new();

        unitManager ??= new();
        unitManager.LoadUnits();

        questManager ??= new();
        questManager.LoadAchievements();
        questManager.LoadQuests();

        dialogManager ??= new();

        Debug.Log(EVENT_TYPE.START);
        Publish(EVENT_TYPE.START);
        Debug.Log(EVENT_TYPE.CONFIGURE);
        Publish(EVENT_TYPE.CONFIGURE);
        playerManager.firstPlay = false;
        IsReady = true;

        if (dialogManager.DialogQueue.Count > 0)
            dialogManager.Start(dialogManager.DialogQueue.Peek());

        Debug.Log(EVENT_TYPE.GAME_READY);
        Publish(EVENT_TYPE.GAME_READY);
    }

    public static void GameQuit()
    {
        Publish(EVENT_TYPE.QUIT);
        SaveManager.SaveGame();
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public static void GamePause()
    {
        Publish(EVENT_TYPE.PAUSE);
        prevTimeScale = Time.timeScale;
        Time.timeScale = 0f;
    }

    public static void GameResume()
    {
        Publish(EVENT_TYPE.RESUME);
        Time.timeScale = prevTimeScale;
    }
}