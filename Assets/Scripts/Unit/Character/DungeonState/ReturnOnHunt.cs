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


        ownerTransform.position += (owner.portalPos - ownerTransform.position).normalized
            * owner.stats.MoveSpeed.Current * Time.deltaTime;
    }

    protected override bool Transition()
    {
        if (Ellipse.IsPointInEllipse(owner.stats.SizeEllipse, owner.portalPos))
        {
            owner.stats.ResetStats();
            controller.ChangeState((int)UnitOnHunt.STATE.IDLE);
            return true;
        }
        return false;
    }
}