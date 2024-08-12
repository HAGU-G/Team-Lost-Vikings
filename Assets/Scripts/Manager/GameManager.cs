//임시로 사용 중

using JetBrains.Annotations;
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
}

public static class GameManager
{
    public static VillageManager villageManager;
    public static HuntZoneManager huntZoneManager;
    public static UIManager uiManager;
    public static UnitManager unitManager = null;
    public static InputManager inputManager;
    public static PlayerManager playerManager = null;
    public static QuestManager questManager = null;
    public static ItemManager itemManager = null;
    public static CameraManager cameraManager;
    public static SoundManager soundManager;

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

        Publish(EVENT_TYPE.LOADED);
        Publish(EVENT_TYPE.INIT);

        SaveManager.LoadGame();

        playerManager ??= new();
        itemManager ??= new();

        unitManager ??= new();
        unitManager.LoadUnits();

        questManager ??= new();
        questManager.LoadAchievements();
        questManager.LoadQuests();

        Publish(EVENT_TYPE.START);
    }

    public static void GameQuit()
    {
        Publish(EVENT_TYPE.QUIT);
        //foreach (var huntZone in huntZoneManager.HuntZones)
        //{
        //    huntZone.Value.EndBossBattle(false);
        //    huntZone.Value.gameObject.SetActive(false);
        //}
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