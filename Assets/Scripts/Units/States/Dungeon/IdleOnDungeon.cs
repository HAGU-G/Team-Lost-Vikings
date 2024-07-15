using System.Globalization;
using UnityEngine;

public class IdleOnDungeon : State<UnitOnDungeon>
{
    private bool isMoving;
    private Vector3 dest;

    public override void EnterState()
    {
        owner.currentState = UnitOnDungeon.STATE.IDLE;
        //owner.spriteRenderer.color = Color.white;
    }

    public override void ExitState()
    {
    }

    public override void ResetState()
    {
    }

    public override void Update()
    {
        //색적
        float maxDepth = float.MinValue;
        foreach (var target in owner.Enemies)
        {
            var depth = Ellipse.CollisionDepth(owner.RecognizeEllipse, target.SizeEllipse);

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

            // TODO 던전 밖으로 이동 못하게 하는 조건으로 대체 ex) 이동 가능 타일 검사
            if (Vector3.Distance(dest, owner.dungeon.transform.position) <= 10f)
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