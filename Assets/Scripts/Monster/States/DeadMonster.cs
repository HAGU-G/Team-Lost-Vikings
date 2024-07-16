﻿using UnityEngine;

public class DeadMonster : State<Monster>
{
    public override void EnterState()
    {
        foreach (var observer in owner.attackers)
        {
            observer.ReceiveNotification(owner, NOTIFY_TYPE.DEAD);
        }

        owner.CurrentHuntZone.Pool.Release(owner);
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