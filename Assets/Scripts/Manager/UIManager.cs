
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum WINDOW_NAME
{
    CHARACTER_INVENTORY,

}


public class UIManager
{
    public GameObject groupBottom;
    public GameObject groupTop;

    public Dictionary<WINDOW_NAME, IWindowable> windows = new();
    public UICharacterInventory chracterInventory;

    /////////////////////////////////////////////////////////////////
    // UI -> Function ///////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////

    public void OnShowCharacter(int instanceID)
    {
        var selectedCharacter = GameManager.unitManager.Units[instanceID];
        Debug.Log(instanceID
            + "\n"
            + selectedCharacter.HP
            + "\n"
            + selectedCharacter.Location);
        if (selectedCharacter.Location != LOCATION.NONE)
            Camera.main.transform.position = selectedCharacter.objectTransform.position + Vector3.forward * -10f;
    }



    /////////////////////////////////////////////////////////////////
    // Function -> UI ///////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////








    /////////////////////////////////////////////////////////////////
    // Windows //////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////

    public void AddWindow(WINDOW_NAME windowName, IWindowable window)
    {
        if (windows.ContainsKey(windowName))
        {
            Debug.Log($"윈도우 이름({windowName})이 이미 등록되어있습니다.");
            return;
        }

        if (windows.ContainsValue(window))
        {
            Debug.LogWarning($"다른 이름으로 등록된 윈도우입니다.", window.gameObject);
            return;
        }

        windows.Add(windowName, window);
    }

    public void CloseWindows(params IWindowable[] exceptWindow)
    {
        var excepts = exceptWindow.ToList();
        foreach (var window in windows)
        {
            if (excepts.Contains(window.Value))
                continue;

            window.Value.Close();
        }
    }
    public void CloseWindows(params WINDOW_NAME[] exceptWindow)
    {
        var excepts = exceptWindow.ToList();
        foreach (var window in windows)
        {
            if (excepts.Contains(window.Key))
                continue;

            window.Value.Close();
        }
    }

}