using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ParameterTypes
{
    NONE = 0,
    HP,
    STAMINA,
}

public class ParameterRecoveryBuilding : MonoBehaviour, IInteractableWithUnit
{
    public Building building;
    public ParameterTypes parameterTypes;
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
