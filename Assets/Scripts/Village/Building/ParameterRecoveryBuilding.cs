using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Building))]
public class ParameterRecoveryBuilding : MonoBehaviour, IInteractableWithUnit
{
    public Building building;
    public PARAMETER_TYPE parameterType;
    private UnitOnVillage unit;
    public List<UnitOnVillage> interactingUnits;
    public List<UnitOnVillage> movingUnits;
    public int recoveryAmount;
    public float recoveryTime;
    public int requireGold;
    //private bool isRecovering;

    public event Action<PARAMETER_TYPE> OnRecoveryDone;
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
        Debug.Log($"hp : {unit.stats.HP} stamina : {unit.stats.Stamina} stress : {unit.stats.Stress}");
        GameManager.cameraManager.FinishFocusOnUnit();
        interactingUnits.Add(unit);
        bool isComplete = false;
        GameManager.uiManager.windows[WINDOW_NAME.PARAMETER_POPUP].GetComponent<UIBuildingParameterPopUp>().SetCharacterInformation();
        yield return new WaitForSeconds(recoveryTime);

        while (true)
        {
            if (unit == null)
            {
                isComplete = true;
            }
            else
            {
                //if(unit.isRecoveryQuited)
                //{
                //    interactingUnits.Remove(unit);
                //    unit.RecoveryAgain(parameterType);
                //    unit.isRecoveryQuited = false;
                //    break;
                //}

                switch (parameterType)
                {
                    case PARAMETER_TYPE.HP:
                        unit.stats.HP.Current += recoveryAmount;
                        GameManager.itemManager.Gold += requireGold;
                        Debug.Log($"hp : {unit.stats.HP.Current}");
                        if (unit.stats.HP.Current < unit.stats.HP.max)
                            yield return new WaitForSeconds(recoveryTime);
                        else if (unit.stats.HP.Current >= unit.stats.HP.max)
                        {
                            unit.stats.HP.Current = unit.stats.HP.max;
                            GameManager.uiManager.windows[WINDOW_NAME.PARAMETER_POPUP].GetComponent<UIBuildingParameterPopUp>().SetParameterBar();
                            yield return new WaitForSeconds(0.1f);
                            isComplete = true;
                        }
                        break;
                    case PARAMETER_TYPE.STAMINA:
                        unit.stats.Stamina.Current += recoveryAmount;
                        GameManager.itemManager.Gold += requireGold;
                        Debug.Log($"stamina : {unit.stats.Stamina}");
                        if (unit.stats.Stamina.Current < unit.stats.Stamina.max)
                            yield return new WaitForSeconds(recoveryTime);
                        else if (unit.stats.Stamina.Current >= unit.stats.Stamina.max)
                        {
                            unit.stats.Stamina.Current = unit.stats.Stamina.max;
                            GameManager.uiManager.windows[WINDOW_NAME.PARAMETER_POPUP].GetComponent<UIBuildingParameterPopUp>().SetParameterBar();
                            yield return new WaitForSeconds(0.1f);
                            isComplete = true;
                        }
                        break;
                    case PARAMETER_TYPE.MENTAL:
                        unit.stats.Stress.Current += recoveryAmount;
                        GameManager.itemManager.Gold += requireGold;
                        Debug.Log($"stress : {unit.stats.Stress}");
                        if (unit.stats.Stress.Current < unit.stats.Stress.max)
                            yield return new WaitForSeconds(recoveryTime);
                        else if (unit.stats.Stress.Current >= unit.stats.Stress.max)
                        {
                            unit.stats.Stress.Current = unit.stats.Stress.max;
                            GameManager.uiManager.windows[WINDOW_NAME.PARAMETER_POPUP].GetComponent<UIBuildingParameterPopUp>().SetParameterBar();
                            yield return new WaitForSeconds(0.1f);
                            isComplete = true;
                        }
                        break;
                }
            }
            if (isComplete)
            {
                //isRecovering = false;
                if (unit == null)
                {
                    RemoveNullInteractingUnits(); 
                }
                else
                {
                    interactingUnits.Remove(unit);
                    unit.RecoveryDone(parameterType);
                }
                //OnRecoveryDone?.Invoke(parameterType);

                yield break;
            }
        }
    }

    private void RemoveNullInteractingUnits()
    {
        for (int i = interactingUnits.Count - 1; i >= 0; i--)
        {
            if (interactingUnits[i] == null)
                interactingUnits.RemoveAt(i);
        }
    }

    private void EnterUnit()
    {

    }

    public void SetUnit(UnitOnVillage unit)
    {
        this.unit = unit;
    }

    public void SetParameter(PARAMETER_TYPE parameterType)
    {
        this.parameterType = parameterType;
    }

    private void RecoveryDone(PARAMETER_TYPE type)
    {
        if (type == parameterType)
        {
            OnRecoveryDone?.Invoke(type);
        }
    }

    public void TouchParameterBuilding()
    {
        if (!GameManager.villageManager.constructMode.isConstructMode)
        {
            GameManager.uiManager.currentParameterBuilding = this;
            GameManager.uiManager.currentNormalBuidling = gameObject.GetComponent<Building>();
            GameManager.villageManager.village.upgrade = gameObject.GetComponent<BuildingUpgrade>();
            GameManager.uiManager.windows[WINDOW_NAME.PARAMETER_POPUP].Open();
        }
        else if (GameManager.villageManager.constructMode.isConstructMode)
        {
            var constructMode = GameManager.uiManager.windows[WINDOW_NAME.CONSTRUCT_MODE] as UIConstructMode;

            if (!constructMode.isConstructing && !constructMode.IsReplacing)
            {
                GameManager.uiManager.currentParameterBuilding = this;
                GameManager.uiManager.currentNormalBuidling = gameObject.GetComponent<Building>();
                GameManager.uiManager.currentBuildingData = GameManager.uiManager.currentNormalBuidling.GetBuildingData();
                GameManager.villageManager.village.upgrade = gameObject.GetComponent<BuildingUpgrade>();
                GameManager.uiManager.uiDevelop.TouchBuildingInConstructMode();
            }
        }

    }

    public void AddMovingUnit(UnitOnVillage unit)
    {
        if (!movingUnits.Contains(unit))
        {
            movingUnits.Add(unit);
        }
    }

    public void RemoveMovingUnit(UnitOnVillage unit)
    {
        if (movingUnits.Contains(unit))
        {
            movingUnits.Remove(unit);
        }
    }

    public void UpdateMovingUnitsDestination()
    {
        for (int i = movingUnits.Count - 1; i >= 0; --i)
        {
            movingUnits[i].UpdateDestination(building.gameObject);
        }
        //foreach(var unit in movingUnits)
        //{
        //    unit.UpdateDestination(building.gameObject);
        //}
    }
}
