//using UnityEngine;

//public class IdleMonster : State<Monster>
//{
//    private bool isMoving;
//    private Vector3 direc;
//    private Vector3 dest;
//    private float roamTimer;

//    public override void EnterState()
//    {
//        owner.currentState = Monster.STATE.IDLE;
//    }

//    public override void ExitState()
//    {
//    }

//    public override void ResetState()
//    {
//        isMoving = false;
//        roamTimer = 0f;
//    }

//    public override void Update()
//    {
//        //색적
//        float maxDepth = float.MinValue;
//        foreach (var target in owner.Enemies)
//        {
//            var depth = Ellipse.CollisionDepth(owner.stats.RecognizeEllipse, target.stats.PresenseEllipse);

//            if (depth >= 0f && depth >= maxDepth)
//            {
//                maxDepth = depth;
//                owner.attackTarget = target;
//            }
//        }

//        //상태 전환
//        if (Transition())
//            return;


//        //배회
//        if (!isMoving)
//        {

//            // TODO 던전 밖으로 이동 못하게 하는 조건으로 대체 ex) 이동 가능 타일 검사

//            dest = owner.transform.position + (Vector3)Random.insideUnitCircle.normalized * owner.stats.MoveSpeed.Current * GameSetting.Instance.monsterRoamSeconds;

//            int count = 0;
//            while (owner.CurrentHuntZone.gridMap.PosToIndex(dest) == Vector2Int.one * -1
//                && count < 10)
//            {
//                dest = owner.transform.position + (Vector3)Random.insideUnitCircle.normalized * owner.stats.MoveSpeed.Current * GameSetting.Instance.monsterRoamSeconds;
//                count++;
//            }

//            if (owner.CurrentHuntZone.gridMap.PosToIndex(dest) == Vector2Int.one * -1)
//                direc = (owner.CurrentHuntZone.transform.position - owner.transform.position).normalized;
//            else
//                direc = (dest - owner.transform.position).normalized;



//            isMoving = true;
//            roamTimer = 0f;
//        }
//        else
//        {
//            if (roamTimer >= GameSetting.Instance.monsterRoamSeconds)
//            {
//                isMoving = false;
//                return;
//            }

//            roamTimer += Time.deltaTime;
//            owner.MoveToDirection(direc, Time.deltaTime);
//        }
//    }

//    protected override bool Transition()
//    {
//        if (owner.HasTarget())
//        {
//            controller.ChangeState((int)UnitOnHunt.STATE.TRACE);
//            return true;
//        }
//        return false;
//    }
//}