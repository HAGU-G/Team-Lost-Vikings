using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class UIUnitsInformation : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.UNITS_INFORMATION;

    public Button exit;

    public Transform content;
    public GameObject unitInfo;
    public List<GameObject> infos;

    private int unitCnt = 0; 
    
    private string path = "Assets/Pick_Asset/2WEEK/GradeIcon/Grade_";
    private List<Sprite> gradeIcons = new();
    protected override void OnGameStart()
    {
        base.OnGameStart();

        for (int i = 1; i <= 5; ++i)
        {
            var newpath = $"{path}{i}.png";
            Addressables.LoadAssetAsync<Sprite>(newpath).Completed += OnLoadDone;
        }
    }

    private void OnLoadDone(AsyncOperationHandle<Sprite> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            gradeIcons.Add(obj.Result);
        }
    }

    private void OnEnable()
    {
        if (!IsReady)
            return;

        SetInfo();
    }

    public void SetInfo()
    {
        for(int i = 0; i < infos.Count; ++i)
        {
            Destroy(infos[i]);
        }
        infos.Clear();

        var units = GameManager.unitManager.Units;
        unitCnt = units.Count;

        foreach(var unit in units)
        {
            var obj = GameObject.Instantiate(unitInfo, content);
            var info = obj.GetComponent<CharacterInfo>();
            info.characterName.text = $"{unit.Value.Data.Name}";
            info.gradeIcon.sprite = gradeIcons[(int)unit.Value.UnitGrade];
            info.information.onClick.AddListener(() =>
            {
                GameManager.uiManager.currentUnitStats = unit.Value;
                OnButtonUnit(unit.Value);
                Close();
            });
            info.characterIcon.uvRect
                = GameManager.uiManager.unitRenderTexture.LoadRenderTexture(unit.Value.Data.UnitAssetFileName);
            info.information.onClick.AddListener(
            () =>
            {
                GameManager.uiManager.currentUnitStats = unit.Value;
                GameManager.uiManager.windows[WINDOW_NAME.UNIT_DETAIL_INFORMATION].Open();
            }
                );
            info.location_state.text = $"";
            info.characterId = unit.Value.InstanceID;
            infos.Add(obj);
        }

        //for (int i = 0; i < units.Count; ++i)
        //{
        //    var button = GameObject.Instantiate(unitInfo, content);
        //    var unit = button.GetComponent<CharacterInfo>();
        //    unit.characterName.text = $"{units.GetValueOrDefault(i).Name}";
        //    unit.characterGrade.text = $"{units.GetValueOrDefault(i).UnitGrade}";
            
        //    unit.information.onClick.AddListener(
        //    () =>
        //    {
        //        GameManager.uiManager.currentUnitStats = units[i];
        //        GameManager.uiManager.windows[(int)WINDOW_NAME.UNIT_DETAIL_INFORMATION].Open();
        //    }
        //        );
        //}
    }

    private void Update()
    {
        if (unitCnt != GameManager.unitManager.Units.Count)
            SetInfo();

        SetUnitLocationState();
    }

    private void SetUnitLocationState()
    {
        foreach(var info in infos)
        {
            var charInfo = info.GetComponent<CharacterInfo>();
            UnitStats unit = GameManager.unitManager.GetUnit(charInfo.characterId);
            string location;
            string state;

            switch(unit.Location)
            {
                case LOCATION.NONE:
                    location = "";
                    state = "";
                    break;
                case LOCATION.VILLAGE:
                    location = "마을";
                    var currVillState = unit.objectTransform.GetComponent<UnitOnVillage>().currentState;
                    switch(currVillState)
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
                    switch(currHuntState)
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

    public void OnButtonUnit(UnitStats unit)
    {
        GameManager.uiManager.windows[WINDOW_NAME.NOTIFICATION].Open();
    }

    public void OnButtonExit()
    {
        Close();
    }
}
