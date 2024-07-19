using UnityEngine;

public class PortalBuilding : MonoBehaviour, IInteractableWithUnit
{
    public void InteractWithUnit(UnitOnVillage unit)
    {
        Debug.Log("GOHUNT");
        GameManager.villageManager.village.GoHunt(unit);
    }
}