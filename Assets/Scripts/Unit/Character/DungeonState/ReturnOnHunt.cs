using UnityEngine;

public class ReturnOnHunt : State<CombatUnit>
{
    Transform ownerTransform;

    public override void EnterState()
    {
        ownerTransform = owner.transform;
        owner.attackTarget = null;
        owner.currentState = CombatUnit.STATE.RETURN;
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

        owner.MoveToDestination(owner.PortalPos, Time.deltaTime);
    }

    protected override bool Transition()
    {
        if (Ellipse.IsPointInEllipse(owner.stats.SizeEllipse, owner.PortalPos)
            && owner.stats.Data.UnitType == UNIT_TYPE.CHARACTER)
        {
            (owner as UnitOnHunt).ReturnToVillage();
            return true;
        }
        return false;
    }
}