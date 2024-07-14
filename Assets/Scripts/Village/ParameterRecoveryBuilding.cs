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
    private Coroutine recoveryCoroutine;

    private void Awake()
    {
        building = GetComponent<Building>();
    }

    public void InteractWithUnit(UnitOnVillage unit)
    {
        SetUnit(unit);
        if (!isRecovering)
        {
            recoveryCoroutine = StartCoroutine(CoRecovery());
        }

    }

    private void Update()
    {
    }

    private IEnumerator CoRecovery()
    {
        isRecovering = true;
        Debug.Log(recoveryTime);
        Debug.Log($"hp : {unit.stats.HP} stamina : {unit.stats.Stamina} stress : {unit.stats.Stress}");
        yield return new WaitForSeconds(recoveryTime);
        bool isComplete = false;

        while (true)
        {
            switch (parameterType)
            {
                case PARAMETER_TYPES.HP:
                    unit.stats.HP += recoveryAmount;
                    Debug.Log($"hp : {unit.stats.HP}");
                    if(unit.stats.HP < unit.stats.CurrentMaxHP)
                        yield return new WaitForSeconds(recoveryTime);
                    else if (unit.stats.HP >= unit.stats.CurrentMaxHP)
                    {
                        unit.stats.HP = unit.stats.CurrentMaxHP;
                        isComplete = true;
                    }
                    break;
                case PARAMETER_TYPES.STAMINA:
                    unit.stats.Stamina += recoveryAmount;
                    Debug.Log($"stamina : {unit.stats.Stamina}");
                    if (unit.stats.Stamina < unit.stats.CurrentStats.MaxStamina)
                        yield return new WaitForSeconds(recoveryTime);
                    else if (unit.stats.Stamina >= unit.stats.CurrentStats.MaxStamina)
                    {
                        unit.stats.Stamina = unit.stats.CurrentStats.MaxStamina;
                        isComplete = true;
                    }
                    break;
                case PARAMETER_TYPES.STRESS:
                    unit.stats.Stress += recoveryAmount;
                    Debug.Log($"stress : {unit.stats.Stress}");
                    if (unit.stats.Stress < unit.stats.CurrentStats.MaxStress)
                        yield return new WaitForSeconds(recoveryTime);
                    else if (unit.stats.Stress >= unit.stats.CurrentStats.MaxStress)
                    {
                        unit.stats.Stress = unit.stats.CurrentStats.MaxStress;
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
