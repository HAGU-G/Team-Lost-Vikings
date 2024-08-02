using UnityEngine;

public class ReturnOnHunt : State<CombatUnit>
{
    Transform ownerTransform;

    public Vector3 entrancePos;

    public override void EnterState()
    {
        ownerTransform = owner.transform;
        owner.attackTarget = null;
        owner.currentState = CombatUnit.STATE.RETURN;

        float compare = 1000f;
        foreach (var entranceTile in owner.CurrentHuntZone.entranceTiles)
        {
            var dis = Vector3.Distance(owner.transform.position, entranceTile.transform.position);
            if(compare > dis)
            {
                compare = dis;
                entrancePos = entranceTile.transform.position;
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

        owner.MoveToDestination(entrancePos, Time.deltaTime);
    }

    protected override bool Transition()
    {
        if (Ellipse.IsPointInEllipse(owner.stats.SizeEllipse, entrancePos)
            && owner.stats.Data.UnitType == UNIT_TYPE.CHARACTER)
        {
            (owner as UnitOnHunt).ReturnToVillage();
            return true;
        }
        return false;
    }
}