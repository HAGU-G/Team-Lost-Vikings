using UnityEngine;

public class IdleOnHunt : State<UnitOnHunt>
{
    private bool isMoving;
    private Vector3 dest;

    public override void EnterState()
    {
        owner.currentState = UnitOnHunt.STATE.IDLE;
        //owner.spriteRenderer.color = Color.white;
    }

    public override void ExitState()
    {
    }

    public override void ResetState()
    {
        isMoving = false;
    }

    public override void Update()
    {
        //색적
        float maxDepth = float.MinValue;
        foreach (var target in owner.Enemies)
        {
            var depth = Ellipse.CollisionDepth(owner.stats.RecognizeEllipse, target.stats.PresenseEllipse);

            if (depth >= 0f && depth >= maxDepth)
            {
                maxDepth = depth;
                owner.attackTarget = target;
            }
        }

        //상태 전환
        if (Transition())
            return;

        //배회
        if (!isMoving)
        {
            dest = owner.transform.position + (Vector3)Random.insideUnitCircle.normalized * owner.stats.MoveSpeed.Current;

            int count = 0;
            while (owner.CurrentHuntZone.gridMap.PosToIndex(dest) == Vector2Int.one * -1
                && count < 10)
            {
                dest = owner.transform.position + (Vector3)Random.insideUnitCircle.normalized * owner.stats.MoveSpeed.Current;
                count++;
            }

            if (owner.CurrentHuntZone.gridMap.PosToIndex(dest) == Vector2Int.one * -1)
                dest = owner.CurrentHuntZone.transform.position;

            isMoving = true;
        }
        else
        {
            owner.transform.position += (dest - owner.transform.position).normalized
                * owner.stats.MoveSpeed.Current * Time.deltaTime;

            if (Vector3.Distance(dest, owner.transform.position) <= 0.2f)
                isMoving = false;
        }
    }

    protected override bool Transition()
    {
        if (owner.HasTarget())
        {
            controller.ChangeState((int)UnitOnHunt.STATE.TRACE);
            return true;
        }
        else if (owner.IsNeedReturn)
        {
            controller.ChangeState((int)UnitOnHunt.STATE.RETURN);
            return true;
        }
        return false;
    }
}