using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using static UnityEditor.FilePathAttribute;
using static UnityEditor.PlayerSettings;
using static UnityEngine.UI.CanvasScaler;

public class UIPlacement : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.PLACEMENT;

    public Button exit;
    
    public Transform huntZoneTransform;
    public Transform ownTransform;

    public TextMeshProUGUI huntZoneName;
    public TextMeshProUGUI ownCount;

    public GameObject unitPrefab;
    
    private Dictionary<int, Sprite> gradeIcons = new();
    private List<GameObject> huntZoneFrames = new();
    private List<GameObject> ownListFrames = new();

    private int currHuntZoneNum = 1;

    protected override void OnGameStart()
    {
        base.OnGameStart();

        var cnt = Enum.GetValues(typeof(UNIT_GRADE)).Length;
        for (int i = 0; i < cnt; ++i)
        {
            var path = $"Grade_0{i + 1}";
            var id = i;
            Addressables.LoadAssetAsync<Sprite>(path).Completed += (obj) => OnLoadDone(obj, id);
        }

        exit.onClick.AddListener(OnButtonExit);
    }

    private void OnLoadDone(AsyncOperationHandle<Sprite> obj, int id)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            gradeIcons.Add(id, obj.Result);
        }
    }

    private void OnEnable()
    {
        SetHuntZoneTransform(currHuntZoneNum);
        SetOwnTransform();
    }

    private void SetHuntZoneTransform(int huntZoneNum)
    {
        var huntZone = GameManager.huntZoneManager.HuntZones[huntZoneNum];
        
        foreach(var frame in huntZoneFrames)
        {
            Destroy(frame);
        }
        huntZoneFrames.Clear();

        foreach(var unit in huntZone.Units)
        {
            huntZoneFrames.Add(InstantiateUnitFrame(unit.stats, huntZoneTransform));
        }
    }

    private void SetOwnTransform()
    {
        foreach (var frame in ownListFrames)
        {
            Destroy(frame);
        }
        ownListFrames.Clear();

        foreach(var unit in GameManager.unitManager.Units.Values)
        {
            if (unit.HuntZoneNum == currHuntZoneNum)
                continue;

            ownListFrames.Add(InstantiateUnitFrame(unit, ownTransform));
        }

        foreach (var unit in GameManager.unitManager.Waitings.Values)
        {
            ownListFrames.Add(InstantiateUnitFrame(unit, ownTransform));
        }

        foreach (var unit in GameManager.unitManager.DeadUnits.Values)
        {
            ownListFrames.Add(InstantiateUnitFrame(unit, ownTransform));
        }
    }

    private GameObject InstantiateUnitFrame(UnitStats unit, Transform content)
    {
        var obj = GameObject.Instantiate(unitPrefab, content);
        var info = obj.GetComponent<CharacterInfo>();
        info.characterName.text = $"{unit.Data.Name}";
        info.gradeIcon.sprite = gradeIcons[(int)unit.UnitGrade];
        info.characterIcon.uvRect
            = GameManager.uiManager.unitRenderTexture.LoadRenderTexture(unit.Data.UnitAssetFileName);
        info.information.onClick.AddListener(
        () =>
        {
            GameManager.uiManager.currentUnitStats = unit;
            GameManager.uiManager.windows[WINDOW_NAME.UNIT_DETAIL_INFORMATION].Open();
        }
            );
        info.location_state.text = $"";
        info.characterId = unit.InstanceID;

        return obj;
    }

    private void Update()
    {
        SetUnitLocationState();
    }

    private void SetUnitLocationState()
    {
        foreach (var info in huntZoneFrames)
        {
            var charInfo = info.GetComponent<CharacterInfo>();
            UnitStats unit = GameManager.unitManager.GetUnit(charInfo.characterId);
            string location;
            string state;

            switch (unit.Location)
            {
                case LOCATION.NONE:
                    location = "";
                    state = "";
                    break;
                case LOCATION.VILLAGE:
                    location = "마을";
                    var currVillState = unit.objectTransform.GetComponent<UnitOnVillage>().currentState;
                    switch (currVillState)
                    {
                        case UnitOnVillage.STATE.IDLE:
                            state = "걷는 중";
                            break;
                        case UnitOnVillage.STATE.GOTO:
                            state = "이동 중";
                            break;
                        case UnitOnVillage.STATE.INTERACT:
                            state = "건물 이용 중";
                            break;
                        case UnitOnVillage.STATE.REVIVE:
                            state = "부활 중";
                            break;
                        default:
                            state = "";
                            break;
                    }
                    break;
                case LOCATION.HUNTZONE:
                    location = "사냥터";
                    var currHuntState = unit.objectTransform.GetComponent<UnitOnHunt>().currentState;
                    switch (currHuntState)
                    {
                        case UnitOnHunt.STATE.IDLE:
                        case UnitOnHunt.STATE.TRACE:
                        case UnitOnHunt.STATE.SKILL:
                        case UnitOnHunt.STATE.ATTACK:
                            state = "전투 중";
                            break;
                        case UnitOnHunt.STATE.DEAD:
                            state = "죽음";
                            break;
                        case UnitOnHunt.STATE.RETURN:
                            state = "복귀 중";
                            break;
                        default:
                            state = "";
                            break;
                    }
                    break;
                default:
                    location = "";
                    state = "";
                    break;
            }
            charInfo.location_state.text = $"{location} - {state}";
        }

        foreach(var info in ownListFrames)
        {
            var charInfo = info.GetComponent<CharacterInfo>();
            UnitStats unit = GameManager.unitManager.GetUnit(charInfo.characterId);
            string location;
            string state;

            switch (unit.Location)
            {
                case LOCATION.NONE:
                    location = "";
                    state = "";
                    break;
                case LOCATION.VILLAGE:
                    location = "마을";
                    var currVillState = unit.objectTransform.GetComponent<UnitOnVillage>().currentState;
                    switch (currVillState)
                    {
                        case UnitOnVillage.STATE.IDLE:
                            state = "걷는 중";
                            break;
                        case UnitOnVillage.STATE.GOTO:
                            state = "이동 중";
                            break;
                        case UnitOnVillage.STATE.INTERACT:
                            state = "건물 이용 중";
                            break;
                        case UnitOnVillage.STATE.REVIVE:
                            state = "부활 중";
                            break;
                        default:
                            state = "";
                            break;
                    }
                    break;
                case LOCATION.HUNTZONE:
                    location = "사냥터";
                    var currHuntState = unit.objectTransform.GetComponent<UnitOnHunt>().currentState;
                    switch (currHuntState)
                    {
                        case UnitOnHunt.STATE.IDLE:
                        case UnitOnHunt.STATE.TRACE:
                        case UnitOnHunt.STATE.SKILL:
                        case UnitOnHunt.STATE.ATTACK:
                            state = "전투 중";
                            break;
                        case UnitOnHunt.STATE.DEAD:
                            state = "죽음";
                            break;
                        case UnitOnHunt.STATE.RETURN:
                            state = "복귀 중";
                            break;
                        default:
                            state = "";
                            break;
                    }
                    break;
                default:
                    location = "";
                    state = "";
                    break;
            }
            charInfo.location_state.text = $"{location} - {state}";
        }
    }

    private void OnButtonExit()
    {
        Close();
    }
}
