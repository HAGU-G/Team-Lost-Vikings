using System.Collections.Generic;
using UnityEngine;

public class UnitOnVillage : Character
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

    //public event Action<PARAMETER_TYPES> OnUnitRecoveryDone;

    public enum STATE
    {
        IDLE,
        GOTO,
        INTERACT,
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
            new InteractOnVillage());
    }

    public override void ResetUnit(UnitStats unitStats)
    {
        base.ResetUnit(unitStats);
        stats.SetLocation(LOCATION.VILLAGE);
        VillageFSM.ResetFSM();
    }

    private void Update()
    {
        VillageFSM.Update();
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
        VillageFSM.ChangeState((int)UnitOnHunt.STATE.IDLE);
        GameManager.uiManager.windows[WINDOW_NAME.PARAMETER_POPUP].GetComponent<UIBuildingParameterPopUp>().SetCharacterInformation();
    }




   
}