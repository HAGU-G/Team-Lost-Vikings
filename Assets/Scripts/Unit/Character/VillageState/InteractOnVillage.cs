using System;
using UnityEngine;

public class InteractOnVillage : State<UnitOnVillage>
{
    private Action OnInteract;
    private IInteractableWithUnit unitInteractBuilding;

    public override void EnterState()
    {
        owner.currentState = UnitOnVillage.STATE.INTERACT;
        //var currentTile = owner.villageManager.GetTile(owner.gameObject.transform.position);
        //var building = currentTile.tileInfo.ObjectLayer.LayerObject;
        Interact(owner.destination);

        OnInteract?.Invoke();
    }

    public override void ExitState()
    {
        //OnInteract = null;

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


        return false;
    }

    private void Interact(GameObject building)
    {
        var buildingComponent = building.GetComponent<Building>();
        if(buildingComponent.interactWithUnit != null)
        {
            buildingComponent.interactWithUnit?.InteractWithUnit(owner);
            var parameterComponent = building.GetComponent<ParameterRecoveryBuilding>();
            if(parameterComponent != null)
                parameterComponent.OnRecoveryDone += owner.RecoveryDone;
        }
        
        buildingComponent.interactWithPlayer?.InteractWithPlayer();
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
