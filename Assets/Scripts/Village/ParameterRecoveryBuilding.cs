using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PARAMETER_TYPES
{
    NONE = 0,
    HP,
    STAMINA,
    STRESS,
}

public class ParameterRecoveryBuilding : MonoBehaviour, IInteractableWithUnit
{
    public Building building;
    public PARAMETER_TYPES parameterTypes;
    public float recoveryAmount;
    public float recoveryTime;

    private void Awake()
    {
        building = GetComponent<Building>();
    }

    public void InteractWithUnit()
    {
        Debug.Log($"{parameterTypes}을 {recoveryAmount} 만큼 회복했습니다.");
    }

    private void Recovery()
    {

    }

    private void EnterUnit()
    {

    }
}
