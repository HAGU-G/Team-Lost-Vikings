using UnityEngine;

public class IdleOnHunt : State<CombatUnit>
{
    private bool isMoving;
    private Vector3 dest;

    public override void EnterState()
    {
        owner.currentState = CombatUnit.STATE.IDLE;
        owner.isTargetFixed = false;
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
            int count = 0;
            Cell cell = null;

            do
            {
                dest = owner.transform.position + (Vector3)Random.insideUnitCircle.normalized * owner.stats.MoveSpeed.Current;
                Vector2Int index = owner.CurrentHuntZone.gridMap.PosToIndex(dest);
                cell = owner.CurrentHuntZone.gridMap.GetTile(index.x, index.y);
                count++;
            }
            while (count <= 10
                   && (cell == null ? true : !cell.tileInfo.ObjectLayer.IsEmpty));

            if (owner.CurrentHuntZone.gridMap.PosToIndex(dest) == Vector2Int.one * -1)
                dest = owner.CurrentHuntZone.transform.position;

            isMoving = true;
        }
        else
        {
            owner.MoveToDestination(dest, Time.deltaTime);
            //owner.transform.position += (dest - owner.transform.position).normalized
            //    * owner.stats.MoveSpeed.Current * Time.deltaTime;

            if (Vector3.Distance(dest, owner.transform.position) <= 0.2f)
                isMoving = false;
        }
    }

    protected override bool Transition()
    {
        if (owner.forceReturn)
        {
            controller.ChangeState((int)UnitOnHunt.STATE.RETURN);
            return true;
        }

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