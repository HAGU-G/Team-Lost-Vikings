//임시로 사용 중

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EventBusType
{
    PAUSE,
    SCENE_CHANGE,
}

public static class GameManager
{ 
    public static VillageManager villageManager;
    public static HuntZoneManager huntZoneManager;
    public static UIManager uiManager;
    public static UnitManager unitManager = new();
    public static InputManager inputManager;
    public static PlayerManager playerManager;
    public static QuestManager questManager;

    private static IDictionary<EventBusType, UnityEvent> events = new Dictionary<EventBusType, UnityEvent>();


    public static void Subscribe(EventBusType eventType, UnityAction listener)
    {
        UnityEvent action;

        if(events.TryGetValue(eventType, out action))
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

    public static void UnSubscribe(EventBusType eventType, in UnityAction listener)
    {
        UnityEvent action;

        if(events.TryGetValue(eventType, out action))
        {
            Debug.LogWarning("해제됨");
            action.RemoveListener(listener);
        }
    }

    public static void Publish(EventBusType eventType)
    {
        UnityEvent action;

        
        if(events.TryGetValue(eventType, out action))
        {
            action.Invoke();
        }
    }



}