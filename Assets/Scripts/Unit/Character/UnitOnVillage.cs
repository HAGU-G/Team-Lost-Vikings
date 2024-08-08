using System.Collections.Generic;
using UnityEngine;

public class UnitOnVillage : Unit
{
    public FSM<UnitOnVillage> VillageFSM {get; private set;}
    public STATE currentState;
    public LACKING_PARAMETER lackParameter;
    public GameObject destination;
    public GameObject forceDestination;
    public Cell destinationTile;
    public List<Cell> destinationTiles = new();
    public VillageManager villageManager;
    public UnitMove unitMove;
    public bool isRecoveryQuited = false;
    public bool isReviveQuited = false;

    //public event Action<PARAMETER_TYPES> OnUnitRecoveryDone;

    public enum STATE
    {
        IDLE,
        GOTO,
        INTERACT,
        REVIVE,
    }

    public enum LACKING_PARAMETER
    {
        HP,
        STAMINA,
        STRESS,
        NONE,
    }

    public override void Init()
    {
        base.Init();
        destinationTile = gameObject.AddComponent<Cell>();
        villageManager = FindObjectOfType<VillageManager>();
        unitMove = GetComponent<UnitMove>();
        VillageFSM = new();
        VillageFSM.Init(this, 0,
            new IdleOnVillage(),
            new GotoOnVillage(),
            new InteractOnVillage(),
            new ReviveOnVillage());
    }

    public override void ResetUnit(UnitStats unitStats)
    {
        base.ResetUnit(unitStats);
        stats.SetLocation(LOCATION.VILLAGE);
        VillageFSM.ResetFSM();
    }

    protected override void Update()
    {
        base.Update();
        VillageFSM.Update();

        if (currentState == STATE.GOTO && destination != null)
        {
            var building = destination.GetComponent<ParameterRecoveryBuilding>();
            if (building != null)
            {
                building.AddMovingUnit(this);
            }
        }
    }

    public List<Cell> FindPath(Cell start, Cell end)
    {
        return villageManager.gridMap.PathFinding(start, end);
    }

    public (List<Cell>, Cell) FindShortestPath(Cell start, List<Cell> entrances)
    {
        return villageManager.gridMap.GetShortestPath(start, entrances);
    }

    public LACKING_PARAMETER CheckParameter()
    {
        if(stats.HP.Ratio < GameSetting.Instance.returnHPRaito)
            return LACKING_PARAMETER.HP;
        else if(stats.Stamina.Ratio < GameSetting.Instance.returnStaminaRaito)
            return LACKING_PARAMETER.STAMINA;
        else if (stats.Stress.Ratio < GameSetting.Instance.returnStressRaito)
            return LACKING_PARAMETER.STRESS;
        else
            return LACKING_PARAMETER.NONE;
    }

    public void RecoveryDone(PARAMETER_TYPE type)
    {
        //OnUnitRecoveryDone?.Invoke(type);
        VillageFSM.ChangeState((int)UnitOnVillage.STATE.IDLE);
        GameManager.uiManager.windows[WINDOW_NAME.PARAMETER_POPUP].GetComponent<UIBuildingParameterPopUp>().SetCharacterInformation();
    }

    public void RecoveryAgain(PARAMETER_TYPE type)
    {
        VillageFSM.ChangeState((int)STATE.IDLE);
        var parameterPopup = GameManager.uiManager.windows[WINDOW_NAME.PARAMETER_POPUP] as UIBuildingParameterPopUp;
        parameterPopup.SetCharacterInformation();
    }

    public void UpdateDestination(GameObject newDestination)
    {
        destination = newDestination;
        if(currentState == STATE.GOTO)
        {
            VillageFSM.ChangeState((int)STATE.IDLE);
        }
    }
}