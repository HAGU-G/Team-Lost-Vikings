using System.Collections.Generic;
using UnityEditor.Build;
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

        //ResetUnit(); >> 제거
        //테스트 코드 제거, 데이터 테이블 연결을 위해 ResetUnit()이 UnitStats를 필요로 하게 됐습니다.
        //null을 넘기면 인스펙터에서 설정된 스탯들을 그대로 사용합니다.
        //아래는 기존대로 동작하게 하기 위해 대체된 코드입니다.
        ResetUnit(null);    //게임에서 돌아다닐 오브젝트 재설정
        stats.ResetStats(); //용병의 스탯을 재설정, 공유할 데이터를 재설정하기 때문에
                            //마을과 사냥터에 모두 반영됩니다.
        //기존 수치가 기억이 안나 BaseHP 200 Stamina 200 Stress 200으로 설정했습니다.
        //미안해요 ㅠㅠ
    }

    public override void Init()
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

    public override void ResetUnit(UnitStats unitStats)
    {
        base.ResetUnit(unitStats);
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