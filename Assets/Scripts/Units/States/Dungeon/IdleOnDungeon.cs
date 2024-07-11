using System.Globalization;
using UnityEngine;

public class IdleOnDungeon : State<UnitOnDungeon>
{
    private bool isMoving;
    private Vector3 dest;

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

        if (Transition())
            return;

        if (!isMoving)
        {
            do
            {
                dest = owner.transform.position + (Vector3)Random.insideUnitCircle.normalized * owner.stats.CurrentStats.MoveSpeed;
            }
            while (Vector3.Distance(dest, owner.dungeon.transform.position) > 10f);

            isMoving = true;
        }

        owner.transform.position += (dest - owner.transform.position).normalized
            * owner.stats.CurrentStats.MoveSpeed * Time.deltaTime;

        if (Vector3.Distance(dest, owner.transform.position) <= 0.2f)
            isMoving = false;
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