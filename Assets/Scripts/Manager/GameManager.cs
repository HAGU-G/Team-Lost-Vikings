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
    QUIT
}

public static class GameManager
{
    public static VillageManager villageManager;
    public static HuntZoneManager huntZoneManager;
    public static UIManager uiManager;
    public static UnitManager unitManager;
    public static InputManager inputManager;
    public static PlayerManager playerManager;
    public static QuestManager questManager;
    public static ItemManager itemManager;
    public static CameraManager cameraManager;

    private static IDictionary<EVENT_TYPE, UnityEvent> events = new Dictionary<EVENT_TYPE, UnityEvent>();


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
        unitManager ??= new();
        playerManager ??= new();
        itemManager ??= new();
        unitManager.LoadUnits();

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
}