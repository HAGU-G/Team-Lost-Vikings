
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager
{
    public GameObject groupBottom;
    public GameObject groupTop;

    public List<IWindowable> windows = new();
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

    public void AddWindow(IWindowable window)
    {
        if (windows.Contains(window))
            return;

        windows.Add(window);
    }

    public void CloseWindows(params IWindowable[] exceptWindow)
    {
        var excepts = exceptWindow.ToList();
        foreach(var window in windows)
        {
            if (excepts.Contains(window))
                continue;

            window.Close();
        }
    }

}