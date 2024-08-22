using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using static UIWindowMessage;

public class UIPlacement : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.PLACEMENT;

    public Button dropdownButton;
    public Button exit;
    
    public Transform huntZoneTransform;
    public Transform ownTransform;

    public TextMeshProUGUI huntZoneName;
    public TextMeshProUGUI ownCount;

    public GameObject huntZoneListPopUp;

    public GameObject unitPrefab;
    public GameObject huntZoneButtonPrefab;

    public Transform huntZoneDropdown;
    
    private Dictionary<int, Sprite> gradeIcons = new();
    private List<GameObject> huntZoneFrames = new();
    private List<GameObject> ownListFrames = new();
    private List<GameObject> huntZoneList = new();

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

        dropdownButton.onClick.AddListener(() =>
        {
            if (huntZoneListPopUp.activeSelf)
                huntZoneListPopUp.SetActive(false);
            else
            {
                huntZoneListPopUp.SetActive(true);
                //CheckHuntZoneAvailable();
            }
        });

        SetDropDownList();
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
        SetText();
        huntZoneListPopUp.SetActive(false);
        SetHuntZoneTransform(currHuntZoneNum);
        SetOwnTransform(currHuntZoneNum);
    }

    private void SetDropDownList()
    {
        var huntZones = GameManager.huntZoneManager.HuntZones;

        for(int i = 0; i < huntZones.Count; ++i)
        {
            var index = i;
            var obj = GameObject.Instantiate(huntZoneButtonPrefab, huntZoneDropdown);
            obj.GetComponent<Button>().onClick.AddListener(() =>
            {
                var requireLv = huntZones[index + 1].GetCurrentData().RequirePlayerLv;

                if (GameManager.playerManager.level > requireLv)
                {
                    currHuntZoneNum = index + 1;
                    SetText();
                    SetHuntZoneTransform(currHuntZoneNum);
                    SetOwnTransform(currHuntZoneNum);
                    huntZoneListPopUp.SetActive(false);
                }
                else
                {
                    var message = GameManager.uiManager.windows[WINDOW_NAME.MESSAGE_POPUP] as UIWindowMessage;
                    message.ShowMessage(
                        $"유저 레벨 {requireLv}에 들어갈 수 있습니다.", 
                        true, 
                        1.2f, 
                        openAnimation: UIWindowMessage.OPEN_ANIMATION.FADEINOUT,
                        closeType: CLOSE_TYPE.TOUCH);
                }
                    
            });
            obj.GetComponentInChildren<TextMeshProUGUI>().text = $"사냥터 {i+1}";
            huntZoneList.Add(obj);
        }
    }

    private void SetText()
    {
        huntZoneName.text = $"사냥터 {currHuntZoneNum}";
        var huntZone = GameManager.huntZoneManager.HuntZones[currHuntZoneNum];
        ownCount.text = $"{huntZone.Units.Count}/{huntZone.GetCurrentData().UnitCapacity}";
    }

    //private void CheckHuntZoneAvailable()
    //{
    //    var huntZones = GameManager.huntZoneManager.HuntZones;
    //    for (int i = 0; i < huntZoneList.Count; ++i)
    //    {
    //        if (GameManager.playerManager.level > huntZones[i + 1].GetCurrentData().RequirePlayerLv)
    //        {
    //            huntZoneList[i].GetComponent<Button>().onClick.RemoveAllListeners();
    //            huntZoneList[i].GetComponent<Button>().onClick.AddListener(() =>
    //            {

    //            });
    //        }
    //        else
    //            huntZoneList[i].GetComponent<Button>().interactable = false;
    //    }
    //}

    private void SetHuntZoneTransform(int huntZoneNum)
    {
        var hm = GameManager.huntZoneManager;
        
        foreach(var frame in huntZoneFrames)
        {
            Destroy(frame);
        }
        huntZoneFrames.Clear();

        foreach(var unit in hm.UnitDeployment[huntZoneNum])
        {
            huntZoneFrames.Add(InstantiateUnitFrame(GameManager.unitManager.GetUnit(unit), huntZoneTransform));
        }

        
    }

    private void SetOwnTransform(int huntZoneNum)
    {
        foreach (var frame in ownListFrames)
        {
            Destroy(frame);
        }
        ownListFrames.Clear();

        foreach(var unit in GameManager.unitManager.Units.Values)
        {
            if(GameManager.huntZoneManager.UnitDeployment[currHuntZoneNum].Contains(unit.InstanceID))
                continue;

            ownListFrames.Add(InstantiateUnitFrame(unit, ownTransform));
        }

        //foreach (var unit in GameManager.unitManager.Waitings.Values)
        //{
        //    if (GameManager.huntZoneManager.UnitDeployment[currHuntZoneNum].Contains(unit.InstanceID))
        //        continue;
        //    ownListFrames.Add(InstantiateUnitFrame(unit, ownTransform));
        //}

        //foreach (var unit in GameManager.unitManager.DeadUnits.Values)
        //{
        //    if (GameManager.huntZoneManager.UnitDeployment[currHuntZoneNum].Contains(unit.InstanceID))
        //        continue;
        //    ownListFrames.Add(InstantiateUnitFrame(unit, ownTransform));
        //}
    }

    private GameObject InstantiateUnitFrame(UnitStats unit, Transform content)
    {
        var obj = GameObject.Instantiate(unitPrefab, content);
        var info = obj.GetComponent<CharacterInfo>();
        info.characterName.text = $"{unit.Data.Name}";
        info.gradeIcon.sprite = gradeIcons[(int)unit.UnitGrade];
        info.characterIcon.uvRect
            = GameManager.uiManager.unitRenderTexture.LoadRenderTexture(unit.Data.UnitAssetFileName);
        info.information.onClick.AddListener(() =>
        {
            GameManager.uiManager.currentUnitStats = unit;
            GameManager.uiManager.windows[WINDOW_NAME.UNIT_DETAIL_INFORMATION].Open();
        });
        info.location_state.text = $"";
        info.characterId = unit.InstanceID;
        info.unitButton.onClick.AddListener(() =>
            {
                if (GameManager.huntZoneManager.IsDeployed(unit.InstanceID, currHuntZoneNum))
                {
                    unit.ResetHuntZone();
                    unit.ForceReturn();
                }
                else
                {
                    unit.SetHuntZone(currHuntZoneNum);
                }
                SetHuntZoneTransform(currHuntZoneNum);
                SetOwnTransform(currHuntZoneNum);
            });

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
                    location = "대기소";
                    state = "대기 중";
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
