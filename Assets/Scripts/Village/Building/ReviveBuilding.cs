using System.Collections;
using UnityEngine;

public class ReviveBuilding : MonoBehaviour, IInteractableWithUnit
{
    public float reviveTime;

    public void InteractWithUnit(UnitOnVillage unit)
    {
        unit.VillageFSM.ChangeState((int)UnitOnVillage.STATE.REVIVE);
    }

    public void TouchReviveBuilding()
    {
        if (!GameManager.villageManager.constructMode.isConstructMode)
        {
            GameManager.uiManager.currentNormalBuidling = gameObject.GetComponent<Building>();
            GameManager.villageManager.village.upgrade = gameObject.GetComponent<BuildingUpgrade>();
            GameManager.uiManager.windows[WINDOW_NAME.REVIVE_POPUP].Open();
        }
        else if (GameManager.villageManager.constructMode.isConstructMode)
        {
            GameManager.uiManager.currentNormalBuidling = gameObject.GetComponent<Building>();
            GameManager.villageManager.village.upgrade = gameObject.GetComponent<BuildingUpgrade>();
            GameManager.uiManager.uiDevelop.TouchBuildingInConstructMode();
        }
    }
}