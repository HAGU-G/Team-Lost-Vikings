using System.Collections.Generic;
using UnityEngine;

public class UnitOnVillage : Unit
{
    private FSM<UnitOnVillage> villageFSM;
    public STATE currentState;
    public LACKING_PARAMETER lackParameter;
    public GameObject destination;
    public Tile destinationTile = new();
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
        if(stats.HP.Current < stats.HP.max * 0.5)
            return LACKING_PARAMETER.HP;
        else if(stats.Stamina.Current < stats.Stamina.max * 0.5)
            return LACKING_PARAMETER.STAMINA;
        else if (stats.Stress.Current < stats.Stress.max * 0.5)
            return LACKING_PARAMETER.STRESS;
        else
            return LACKING_PARAMETER.NONE;
    }

    //-------Test용 메소드-----------------------------------------------
    

    private void ReduceHp()
    {
        stats.HP.Current = 30;
    }
    public void ReduceStamina()
    {
        stats.Stamina.Current = 30;
    }

    private void ReduceStress()
    {
        stats.Stress.Current = 30;
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(800f, 0f, 100f, 70f), "ReduceHp"))
        {
            ReduceHp();
        }

        if (GUI.Button(new Rect(800f, 100f, 100f, 70f), "ReduceStamina"))
        {
            ReduceStamina();
        }

        if (GUI.Button(new Rect(800f, 200f, 100f, 70f), "ReduceStress"))
        {
            ReduceStress();
        }
    }
    //------------------------------------------------------------------
}