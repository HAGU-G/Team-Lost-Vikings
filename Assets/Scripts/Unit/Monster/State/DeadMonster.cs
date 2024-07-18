using UnityEngine;

public class DeadMonster : State<Monster>
{
    public override void EnterState()
    {
        owner.currentState = Monster.STATE.DEAD;
        owner.SendNotification(NOTIFY_TYPE.DEAD, true);
        owner.DropItem();
        GameManager.huntZoneManager.ReleaseMonster(owner);
    }

    public override void ExitState()
    {
    }

    public override void ResetState()
    {
    }

    public override void Update()
    {
    }

    protected override bool Transition()
    {
        return false;
    }
}