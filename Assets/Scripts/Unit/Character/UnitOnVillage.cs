using System;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;

public class UnitOnVillage : Unit
{
    public FSM<UnitOnVillage> VillageFSM {get; private set;}
    public STATE currentState;
    public LACKING_PARAMETER lackParameter;
    public GameObject destination;
    public GameObject forceDestination;
    public Cell destinationTile;
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
        unitStats.SetLocation(LOCATION.VILLAGE);
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