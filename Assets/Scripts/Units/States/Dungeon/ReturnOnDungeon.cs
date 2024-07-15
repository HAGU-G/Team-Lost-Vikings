using UnityEngine;

public class ReturnOnDungeon : State<UnitOnDungeon>
{
    private float recoveryTimer;
    Transform ownerTransform;

    public override void EnterState()
    {
        ownerTransform = owner.transform;
        owner.attackTarget = null;
        owner.currentState = UnitOnDungeon.STATE.RETURN;
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


        ownerTransform.position += (owner.destinationPos - ownerTransform.position).normalized
            * owner.stats.MoveSpeed.Current * Time.deltaTime;
    }

    protected override bool Transition()
    {
        if (Ellipse.IsPointInEllipse(owner.stats.SizeEllipse, owner.destinationPos))
        {
            owner.stats.ResetStats();
            controller.ChangeState((int)UnitOnDungeon.STATE.IDLE);
            return true;
        }
        return false;
    }
}