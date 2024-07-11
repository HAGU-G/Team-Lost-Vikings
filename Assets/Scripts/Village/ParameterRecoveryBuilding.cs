using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterRecoveryBuilding : MonoBehaviour, IInteractableWithUnit
{
    public Building building;
    public PARAMETER_TYPES parameterType;
    private UnitOnVillage unit;
    public int recoveryAmount;
    public float recoveryTime;
    private bool isRecovering;

    public event Action<PARAMETER_TYPES> OnRecoveryDone;
    private IEnumerator recoveryCoroutine;

    private void Awake()
    {
        building = GetComponent<Building>();
        recoveryCoroutine = CoRecovery();
    }

    public void InteractWithUnit(UnitOnVillage unit)
    {
        SetUnit(unit);
        if (!isRecovering)
        {
            StartCoroutine(recoveryCoroutine);
        }

    }

    private void Update()
    {
    }

    private IEnumerator CoRecovery()
    {
        isRecovering = true;
        Debug.Log(recoveryTime);
        Debug.Log($"hp : {unit.stats.CurrentHP}");
        yield return new WaitForSeconds(recoveryTime);
        bool isComplete = false;

        while (true)
        {
            switch (parameterType)
            {
                case PARAMETER_TYPES.HP:
                    unit.stats.CurrentHP += recoveryAmount;
                    Debug.Log($"hp : {unit.stats.CurrentHP}");
                    if(unit.stats.CurrentHP < unit.stats.CurrentMaxHP)
                        yield return new WaitForSeconds(recoveryTime);
                    else if (unit.stats.CurrentHP >= unit.stats.CurrentMaxHP)
                    {
                        unit.stats.CurrentHP = unit.stats.CurrentMaxHP;
                        isComplete = true;
                    }
                    break;
                case PARAMETER_TYPES.STAMINA:
                    unit.stats.CurrentStamina += recoveryAmount;
                    if (unit.stats.CurrentHP < unit.stats.CurrentStats.MaxStamina)
                        yield return new WaitForSeconds(recoveryTime);
                    else if (unit.stats.CurrentStamina >= unit.stats.CurrentStats.MaxStamina)
                    {
                        unit.stats.CurrentStamina = unit.stats.CurrentStats.MaxStamina;
                        isComplete = true;
                    }
                    break;
                case PARAMETER_TYPES.STRESS:
                    unit.stats.CurrentStress += recoveryAmount;
                    if (unit.stats.CurrentHP < unit.stats.CurrentStats.MaxStress)
                        yield return new WaitForSeconds(recoveryTime);
                    else if (unit.stats.CurrentStress >= unit.stats.CurrentStats.MaxStress)
                    {
                        unit.stats.CurrentStress = unit.stats.CurrentStats.MaxStress;
                        isComplete = true;
                    }
                    break;
            }
            if (isComplete)
            {
                isRecovering = false;
                OnRecoveryDone?.Invoke(parameterType);
                OnRecoveryDone = null;
                yield break;
            }
        }
    }

    private void EnterUnit()
    {

    }

    public void SetUnit(UnitOnVillage unit)
    {
        this.unit = unit;
    }

    public void SetParameter(PARAMETER_TYPES parameterType)
    {
        this.parameterType = parameterType;
    }
}
