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

        recoveryCoroutine = StartCoroutine(CoRecovery(unit));

    }

    private void Update()
    {
    }

    private IEnumerator CoRecovery(UnitOnVillage unit)
    {
        //isRecovering = true;
        Debug.Log(recoveryTime);
        Debug.Log($"hp : {unit.stats.HP} stamina : {unit.stats.Stamina} stress : {unit.stats.Stress}");
        yield return new WaitForSeconds(recoveryTime);
        bool isComplete = false;

        while (true)
        {
            switch (parameterType)
            {
                case PARAMETER_TYPES.HP:
                    unit.stats.HP.Current += recoveryAmount;
                    Debug.Log($"hp : {unit.stats.HP.Current}");
                    if (unit.stats.HP.Current < unit.stats.HP.max)
                        yield return new WaitForSeconds(recoveryTime);
                    else if (unit.stats.HP.Current >= unit.stats.HP.max)
                    {
                        unit.stats.HP.Current = unit.stats.HP.max;
                        isComplete = true;
                    }
                    break;
                case PARAMETER_TYPES.STAMINA:
                    unit.stats.Stamina.Current += recoveryAmount;
                    Debug.Log($"stamina : {unit.stats.Stamina}");
                    if (unit.stats.Stamina.Current < unit.stats.Stamina.max)
                        yield return new WaitForSeconds(recoveryTime);
                    else if (unit.stats.Stamina.Current >= unit.stats.Stamina.max)
                    {
                        unit.stats.Stamina.Current = unit.stats.Stamina.max;
                        isComplete = true;
                    }
                    break;
                case PARAMETER_TYPES.STRESS:
                    unit.stats.Stress.Current += recoveryAmount;
                    Debug.Log($"stress : {unit.stats.Stress}");
                    if (unit.stats.Stress.Current < unit.stats.Stress.max)
                        yield return new WaitForSeconds(recoveryTime);
                    else if (unit.stats.Stress.Current >= unit.stats.Stress.max)
                    {
                        unit.stats.Stress.Current = unit.stats.Stress.max;
                        isComplete = true;
                    }
                    break;
            }
            if (isComplete)
            {
                isRecovering = false;
                unit.RecoveryDone(parameterType);
                OnRecoveryDone?.Invoke(parameterType);
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

    private void RecoveryDone(PARAMETER_TYPES type)
    {
        if(type == parameterType)
        {
            OnRecoveryDone?.Invoke(type);
        }
    }
}
