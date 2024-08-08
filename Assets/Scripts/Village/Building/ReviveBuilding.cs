using System.Collections;
using UnityEngine;

public class ReviveBuilding : MonoBehaviour, IInteractableWithUnit
{
    public float reviveTime;

    public void InteractWithUnit(UnitOnVillage unit)
    {
        unit.VillageFSM.ChangeState((int)UnitOnVillage.STATE.REVIVE);
    }
}