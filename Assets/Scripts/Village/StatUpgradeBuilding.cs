using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StatUpgradeBuilding : MonoBehaviour, IInteractableWithPlayer
{
    public Building building;
    public STATTYPES risingStat; //나중에 데이터형 수정하기
    public int risingValue;

    private void Awake()
    {
        building = GetComponent<Building>();
    }

    public void InteractWithPlayer()
    {
        //UI 띄우기
    }

    private void RiseStat()
    {
        switch (risingStat)
        {
            case STATTYPES.STR:
                break;
            case STATTYPES.MAG:
                break;
            case STATTYPES.AGI:
                break;
        }
    }
}
