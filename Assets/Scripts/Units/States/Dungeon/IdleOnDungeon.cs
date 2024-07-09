using UnityEngine;

public class IdleOnDungeon : State<UnitOnDungeon>
{
    public override void EnterState()
    {
        owner.currentState = UnitOnDungeon.STATE.IDLE;
        owner.spriteRenderer.color = Color.white;
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

        float min = float.MaxValue;
        foreach (var target in owner.Enemies)
        {
            var d = Vector3.Distance(target.transform.position, owner.transform.position) - target.stats.CurrentStats.UnitSize;
            if (d <= owner.stats.CurrentStats.RecognizeRange && d < min)
            {
                min = d;
                owner.attackTarget = target;
            }
        }
    }

    protected override bool Transition()
    {
        if (owner.attackTarget != null)
        {
            controller.ChangeState((int)UnitOnDungeon.STATE.TRACE);
            return true;
        }
        else if (owner.IsNeedReturn)
        {
            controller.ChangeState((int)UnitOnDungeon.STATE.RETURN);
            return true;
        }
        return false;
    }
}