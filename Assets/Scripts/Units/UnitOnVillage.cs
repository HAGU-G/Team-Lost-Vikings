using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class UnitOnVillage : Unit
{
    private FSM<UnitOnVillage> villageFSM;
    public STATE currentState;
    public Tile destination = new();
    public VillageManager villageManager;
    public UnitMove unitMove;

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

    private void Awake()
    {
        Init();
        ResetUnit();
    }

    protected override void Init()
    {
        base.Init();
        villageManager = FindObjectOfType<VillageManager>();
        unitMove = GetComponent<UnitMove>();
        villageFSM = new();
        villageFSM.Init(this, 0,
            new IdleOnVillage(),
            new GotoOnVillage(),
            new InteractOnVillage());
    }

    protected override void ResetUnit()
    {
        base.ResetUnit();
        villageFSM.ResetFSM();
    }

    private void Update()
    {
        villageFSM.Update();
    }

    public List<Tile> FindPath(Tile start, Tile end)
    {
        return villageManager.gridMap.PathFinding(start, end);
    }

    public LACKING_PARAMETER CheckParameter()
    {
        if(stats.CurrentHP < stats.CurrentMaxHP * 0.5)
            return LACKING_PARAMETER.HP;
        else if(stats.CurrentStamina < stats.CurrentStats.MaxStamina * 0.5)
            return LACKING_PARAMETER.STAMINA;
        else if (stats.CurrentStress < stats.CurrentStats.MaxStress * 0.5)
            return LACKING_PARAMETER.STRESS;
        else
            return LACKING_PARAMETER.NONE;
    }

    //-------Test용 메소드-----------------------------------------------
    public void ReduceStamina()
    {
        stats.CurrentStamina -= 70;
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0f, 160f, 70f, 70f), "ReduceStamina"))
        {
            ReduceStamina();
        }
    }
    //------------------------------------------------------------------
}