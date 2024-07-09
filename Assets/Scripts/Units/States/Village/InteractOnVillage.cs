using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractOnVillage : State<UnitOnVillage>
{
    public override void EnterState()
    {
        owner.currentState = UnitOnVillage.STATE.INTERACT;
    }

    public override void ExitState()
    {
        
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
}
