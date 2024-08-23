using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractOnVillage : State<UnitOnVillage>
{
    private Action OnInteract;
    private IInteractableWithUnit unitInteractBuilding;
    private List<SpriteRenderer> renderers = new();

    public override void EnterState()
    {
        owner.currentState = UnitOnVillage.STATE.INTERACT;
        //var currentTile = owner.villageManager.GetTile(owner.gameObject.transform.position);
        //var building = currentTile.tileInfo.ObjectLayer.LayerObject;
        Interact(owner.destination);        
        
        renderers = owner.gameObject.GetComponentsInChildren<SpriteRenderer>().ToList();
        foreach (var render in renderers)
        {
            render.enabled = false;
        }


        OnInteract?.Invoke();
    }

    public override void ExitState()
    {
        foreach (var render in renderers)
        {
            render.enabled = true;
        }
    }

    public override void ResetState()
    {
        
    }

    public override void Update()
    {
        if (Transition())
            return;
    }

    protected override bool Transition()
    {
        if (owner.isRecoveryQuited)
        {
            controller.ChangeState((int)UnitOnVillage.STATE.IDLE);
            return true;
        }

        return false;
    }

    private void Interact(GameObject building)
    {
        var buildingComponent = building.GetComponent<Building>();
        if(buildingComponent.interactWithUnit != null)
        {
            buildingComponent.interactWithUnit?.InteractWithUnit(owner);
            var parameterComponent = building.GetComponent<ParameterRecoveryBuilding>();
            //if(parameterComponent != null)
            //  parameterComponent.OnRecoveryDone += owner.RecoveryDone;
        }
        return ;
    }

    public void RecoveryDone(PARAMETER_TYPE type)
    {
        //owner.RecoveryDone(type);
        switch(type)
        {
            case PARAMETER_TYPE.HP:
                if(owner.stats.HP.Current  == owner.stats.HP.max)
                    controller.ChangeState((int)UnitOnVillage.STATE.IDLE);
                break;
            case PARAMETER_TYPE.STAMINA:
                if (owner.stats.Stamina.Current == owner.stats.Stamina.max)
                    controller.ChangeState((int)UnitOnVillage.STATE.IDLE);
                break;
            case PARAMETER_TYPE.MENTAL:
                if (owner.stats.Stress.Current == owner.stats.Stress.max)
                    controller.ChangeState((int)UnitOnVillage.STATE.IDLE);
                break;
        }
    }
}
