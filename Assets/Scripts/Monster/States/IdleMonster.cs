using UnityEngine;

public class IdleMonster : State<Monster>
{
    private bool isMoving;
    private Vector3 direc;
    private float roamTimer;

    public override void EnterState()
    {
        owner.currentState = Monster.STATE.IDLE;
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
            var dest = owner.transform.position + (Vector3)Random.insideUnitCircle.normalized * owner.stats.MoveSpeed.Current;

            // TODO 던전 밖으로 이동 못하게 하는 조건으로 대체 ex) 이동 가능 타일 검사
            if (Vector3.Distance(dest, owner.dungeon.transform.position) <= 10f)
                direc = (dest - owner.transform.position).normalized;
            else
                direc = (Vector3.zero - owner.transform.position).normalized;

            isMoving = true;
            roamTimer = 0f;
        }
        else
        {
            if (roamTimer >= GameSetting.Instance.monsterRoamTime)
            {
                isMoving = false;
                return;
            }

            roamTimer += Time.deltaTime;

            owner.transform.position += direc * owner.stats.MoveSpeed.Current * Time.deltaTime;
        }
    }

    protected override bool Transition()
    {
        if (owner.attackTarget != null)
        {
            controller.ChangeState((int)UnitOnDungeon.STATE.TRACE);
            return true;
        }
        return false;
    }
}