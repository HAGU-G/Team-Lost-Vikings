using UnityEngine;


public class StatUpgradeBuilding : MonoBehaviour, IInteractableWithPlayer
{
    public Building building;
    public STAT_TYPES risingStat; //나중에 데이터형 수정하기
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
            case STAT_TYPES.STR:
                break;
            case STAT_TYPES.MAG:
                break;
            case STAT_TYPES.AGI:
                break;
        }
    }
}
