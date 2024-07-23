
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.WSA;

public enum WINDOW_NAME
{
    TUTORIAL_POPUP,
    BUILDING_POPUP,
    PARAMETER_POPUP,
    WAIT_FOR_CBT,
    GACHA_UI,
    GACHA_RESULT,
    CHARACTER_STASH,
    UNITS_INFORMATION,
    NOTIFICATION,
    UNIT_DETAIL_INFORMATION,
    CHARACTER_INVENTORY,
    CHARACTER_MANAGEMENT,
}


public class UIManager : MonoBehaviour
{
    public GameObject groupBottom;
    public GameObject groupTop;

    public List<UIWindow> windows;
    public UICharacterInventory chracterInventory;
    public UICharacterWaiting chracterWaiting;
    public UIRenderTexture unitRenderTexture;

    public Building currentNormalBuidling;
    public ParameterRecoveryBuilding currentParameterBuilding;

    public bool isWindowOn = false;

    /////////////////////////////////////////////////////////////////
    // UI -> Function ///////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////

    private void Awake()
    {
        if (GameManager.uiManager != null)
        {
            Destroy(gameObject);
            return;
        }

        GameManager.uiManager = this;
    }

    public void OnShowCharacter(int instanceID)
    {
        var selectedCharacter = GameManager.unitManager.GetUnit(instanceID);
        Debug.Log(instanceID
            + "\n"
            + selectedCharacter.HP
            + "\n"
            + selectedCharacter.Location);
        if (selectedCharacter.Location != LOCATION.NONE)
            Camera.main.transform.position = selectedCharacter.objectTransform.position + Vector3.forward * -10f;
    }
    public void OnPickUpCharacter(int instanceID)
    {
        GameManager.unitManager.PickUpCharacter(instanceID);
        chracterInventory.LoadCharacterButtons(GameManager.unitManager.Units);
    }

    public void OnSetUnitHuntZone(int instanceID, int huntZoneID)
    {
        var unit = GameManager.unitManager.GetUnit(instanceID);
        unit.SetHuntZone(huntZoneID);
    }

    /////////////////////////////////////////////////////////////////
    // Function -> UI ///////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////

    






    /////////////////////////////////////////////////////////////////
    // Windows //////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////

    public void AddWindow(WINDOW_NAME windowName, UIWindow window)
    {
        if (windows.Contains(window))
        {
            Debug.Log($"윈도우 이름({windowName})이 이미 등록되어있습니다.");
            return;
        }
                
        windows.Add(window);
    }

    public void CloseWindows(params UIWindow[] exceptWindow)
    {
        var excepts = exceptWindow.ToList();
        foreach (var window in windows)
        {
            if (excepts.Contains(window))
                continue;

            window.Close();
        }
    }
    //public void CloseWindows(params WINDOW_NAME[] exceptWindow)
    //{
    //    var excepts = exceptWindow.ToList();
    //    foreach (var window in windows)
    //    {
    //        if (excepts.Contains(window))
    //            continue;

    //        window.Close();
    //    }
    //}

}