using UnityEngine;

public class ReturnOnHunt : State<UnitOnHunt>
{
    Transform ownerTransform;

    public override void EnterState()
    {
        ownerTransform = owner.transform;
        owner.attackTarget = null;
        owner.currentState = UnitOnHunt.STATE.RETURN;
        //owner.spriteRenderer.color = Color.black;
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
        //ownerTransform.position += (owner.PortalPos - ownerTransform.position).normalized
        //    * owner.stats.MoveSpeed.Current * Time.deltaTime;
    }

    protected override bool Transition()
    {
        if (Ellipse.IsPointInEllipse(owner.stats.SizeEllipse, owner.PortalPos))
        {
            owner.ReturnToVillage();
            return true;
        }
        return false;
    }
}