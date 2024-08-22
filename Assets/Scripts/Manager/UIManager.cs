
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    CHARACTER_LOCATE,
    CONSTRUCT_MODE,
    BUILDING_DETAIL,
    REVIVE_POPUP,
    TOUCH_UNIT_BUTTONS,
    OPTION,
    ACCOUNT,
    HUNTZONE_DETAIL,
    MESSAGE_POPUP,
    PLACEMENT,
}


public class UIManager : MonoBehaviour
{
    public UIDevelop uiDevelop;
    public GameObject groupBottom;
    public GameObject groupTop;

    public Dictionary<WINDOW_NAME, UIWindow> windows = new();
    public UICharacterInventory chracterInventory;
    public UICharacterWaiting chracterWaiting;
    public UIRenderTexture unitRenderTexture;

    public Building currentNormalBuidling;
    public ParameterRecoveryBuilding currentParameterBuilding;
    public UnitStats currentUnitStats;
    public BuildingData currentBuildingData;

    public bool isWindowOn = false;

    public Sprite[] gradeIcons;
    public Sprite[] tutorialPages;

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
        if (windows.ContainsKey(windowName))
        {
            Debug.Log($"윈도우 이름({windowName})이 이미 등록되어있습니다.");
            return;
        }
                
        windows.Add(windowName, window);
    }

    public void CloseWindows(params UIWindow[] exceptWindow)
    {
        var excepts = exceptWindow.ToList();
        foreach (var window in windows)
        {
            if (excepts.Contains(window.Value))
                continue;

            window.Value.Close();
        }
    }

    public void OpenWindow(WINDOW_NAME windowName)
    {
        if (windows.ContainsKey(windowName))
        {
            UIWindow windowToOpen = windows[windowName];

            // ConstructMode를 제외한 다른 창이 열릴 때 ConstructMode 종료
            if (windowName != WINDOW_NAME.CONSTRUCT_MODE && windowName != WINDOW_NAME.BUILDING_DETAIL)
            {
                var constructModeWindow = windows[WINDOW_NAME.CONSTRUCT_MODE] as UIConstructMode;
                if (constructModeWindow != null && GameManager.villageManager.constructMode.isConstructMode)
                {
                    constructModeWindow.FinishConstructMode();
                }
            }
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