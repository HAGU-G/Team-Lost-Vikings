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

        float compare = 1000f;
        foreach (var entranceTile in owner.CurrentHuntZone.entranceTiles)
        {
            var dis = Vector3.Distance(owner.transform.position, entranceTile.transform.position);
            if(compare > dis)
            {
                compare = dis;
                owner.entrancePos = entranceTile.transform.position;
            }
        }
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

        owner.MoveToDestination(owner.entrancePos, Time.deltaTime);
        //ownerTransform.position += (owner.PortalPos - ownerTransform.position).normalized
        //    * owner.stats.MoveSpeed.Current * Time.deltaTime;
    }

    protected override bool Transition()
    {
        if (Ellipse.IsPointInEllipse(owner.stats.SizeEllipse, owner.entrancePos))
        {
            owner.ReturnToVillage();
            return true;
        }
        return false;
    }
}