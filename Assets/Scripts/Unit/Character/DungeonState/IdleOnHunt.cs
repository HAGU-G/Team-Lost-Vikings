using UnityEngine;

public class IdleOnHunt : State<CombatUnit>
{
    private bool isMoving;
    private Vector3 dest;
    private float timer;

    public override void EnterState()
    {
        owner.currentState = CombatUnit.STATE.IDLE;
        owner.isTargetFixed = false;
        timer = 0f;
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
        var enemies = owner.GetCollidedUnit(owner.stats.RecognizeEllipse, owner.Enemies.ToArray());
        if (enemies.Count > 0)
            owner.attackTarget = enemies[0].Item1;

        //상태 전환
        if (Transition())
            return;

        //배회
        if (!isMoving)
        {
            int count = 0;
            Cell cell = null;
            var gridMap = owner.CurrentHuntZone.gridMap;
            do
            {
                dest = owner.transform.position + (Vector3)Random.insideUnitCircle.normalized * owner.stats.MoveSpeed.Current;
                Vector2Int index = owner.CurrentHuntZone.gridMap.PosToIndex(dest);
                cell = owner.CurrentHuntZone.gridMap.GetTile(index.x, index.y);
                count++;
            }
            while (count <= 3
                   && (cell == null || !cell.tileInfo.ObjectLayer.IsEmpty
                        || !gridMap.usingTileList.Exists((x) =>
                        {
                            return x.tileInfo.id == gridMap.PosToIndex(cell.transform.position);
                        })));

            if (owner.CurrentHuntZone.gridMap.PosToIndex(dest) == Vector2Int.one * -1)
                dest = owner.CurrentHuntZone.transform.position;

            isMoving = true;
        }
        else
        {
            var deltatTime = Time.deltaTime;
            timer += deltatTime;
            owner.MoveToDestination(dest, deltatTime);
            //owner.transform.position += (dest - owner.transform.position).normalized
            //    * owner.stats.MoveSpeed.Current * Time.deltaTime;

            if (Vector3.Distance(dest, owner.transform.position) <= 0.2f
                || timer >= GameSetting.Instance.idleRerouteTime)
            {
                isMoving = false;
                timer = 0f;
            }
        }
    }

    protected override bool Transition()
    {
        if (owner.forceReturn)
        {
            controller.ChangeState((int)CombatUnit.STATE.RETURN);
            return true;
        }

        if (owner.HasTarget())
        {
            controller.ChangeState((int)CombatUnit.STATE.TRACE);
            return true;
        }
        else if (owner.IsNeedReturn)
        {
            controller.ChangeState((int)CombatUnit.STATE.RETURN);
            return true;
        }
        return false;
    }
}