using UnityEngine;
using UnityEngine.UI;
using static UIWindowMessage;

public class UICharacterLocate : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.CHARACTER_LOCATE;

    private UnitStats unit;
    public Button village;
    public Button hpRecovery;
    public Button staminaRecovery;
    public Button stressRecovery;

    public Button exit;

    public Button[] huntzones;

    public GameObject locationPrefab;
    public Transform content;


    protected override void OnGameStart()
    {
        base.OnGameStart();

        village.onClick.AddListener(() =>
        {
            GameManager.cameraManager.FinishFocusOnUnit();
            unit.ResetHuntZone();
            unit.ForceReturn();
            Close();
        });

        hpRecovery.onClick.AddListener(() =>
        {
            GameManager.cameraManager.FinishFocusOnUnit();
            unit.ForceReturn();
            unit.parameterType = PARAMETER_TYPE.HP;
            if (unit.Location == LOCATION.HUNTZONE)
                unit.ArriveVillage += VillageAraive;
            else
                VillageAraive();
            Close();
        });

        staminaRecovery.onClick.AddListener(() =>
        {
            GameManager.cameraManager.FinishFocusOnUnit();
            unit.ForceReturn();
            unit.parameterType = PARAMETER_TYPE.STAMINA;
            if (unit.Location == LOCATION.HUNTZONE)
                unit.ArriveVillage += VillageAraive;
            else
                VillageAraive();
            Close();
        });

        stressRecovery.onClick.AddListener(() =>
        {
            GameManager.cameraManager.FinishFocusOnUnit();
            unit.ForceReturn();
            unit.parameterType = PARAMETER_TYPE.MENTAL;
            if (unit.Location == LOCATION.HUNTZONE)
                unit.ArriveVillage += VillageAraive;
            else
                VillageAraive();
            Close();
        });

        var huntzones = GameManager.huntZoneManager.HuntZones;
        for(int i = 0; i < huntzones.Count; ++i)
        {
            int index = i;
            GameManager.cameraManager.FinishFocusOnUnit();
            int huntzoneNum = i;
            var location = Instantiate(locationPrefab, content);
            var locationComponent = location.GetComponent<Location>();
            locationComponent.locationName.text = $"{huntzoneNum + 1}번 사냥터";
            locationComponent.button.onClick.AddListener(() =>
            {
                var requireLv = GameManager.huntZoneManager.HuntZones[index + 1].GetCurrentData().RequirePlayerLv;
                if (GameManager.playerManager.level < requireLv)
                {
                    var message = GameManager.uiManager.windows[WINDOW_NAME.MESSAGE_POPUP] as UIWindowMessage;
                    message.ShowMessage(
                        $"유저 레벨 {requireLv}에 들어갈 수 있습니다.",
                        true,
                        1.2f,
                        openAnimation: UIWindowMessage.OPEN_ANIMATION.FADEINOUT,
                        closeType: CLOSE_TYPE.TOUCH);
                    return;
                }

                    GameManager.cameraManager.FinishFocusOnUnit();
                
                if (GameManager.huntZoneManager.IsDeployed(unit.InstanceID, huntzoneNum + 1))
                    return;
                SetUnitHuntZone(GameManager.huntZoneManager.HuntZones[huntzoneNum + 1].Info.HuntZoneNum);

                Close();
            });
        }

        exit.onClick.AddListener(OnButtonExit);
    }

    private void OnEnable()
    {
        if (!IsReady)
            return;
        unit = GameManager.uiManager.currentUnitStats;

        foreach (var building in GameManager.villageManager.constructedBuildings)
        {
            var id = building.GetComponent<Building>().StructureId;
            if ((int)STRUCTURE_ID.HP_RECOVERY == id)
                hpRecovery.gameObject.SetActive(true);
            if ((int)STRUCTURE_ID.STAMINA_RECOVERY == id)
                staminaRecovery.gameObject.SetActive(true);
            if ((int)STRUCTURE_ID.STRESS_RECOVERY == id)
                stressRecovery.gameObject.SetActive(true);
        }
    }

    public void OnButtonExit()
    {
        GameManager.cameraManager.FinishFocusOnUnit();
        Close();
    }

    public void OnButtonHp()
    {
        
    }

    public void SetUnitRecoveryBuilding(int recovery)
    {

    }

    public void SetUnitHuntZone(int huntzone)
    {
        unit.SetHuntZone(huntzone);
    }

    public void VillageAraive()
    {
        unit.ArriveVillage -= VillageAraive;
        Debug.Log("이벤트 함수 실행");

        var unitOnVillage = unit.objectTransform.GetComponent<UnitOnVillage>();

        if (unitOnVillage.currentState == UnitOnVillage.STATE.REVIVE)
            return;

            foreach (var building in GameManager.villageManager.constructedBuildings)
        {
            if (building.GetComponent<ParameterRecoveryBuilding>() == null)
                continue;
            if (building.GetComponent<ParameterRecoveryBuilding>().parameterType == unit.parameterType)
            {
                unitOnVillage.forceDestination = building;
                break;
            }
        }
        unitOnVillage.VillageFSM.ChangeState((int)UnitOnVillage.STATE.GOTO);
    }
}