﻿//using UnityEngine;

//public class TraceMonster : State<Monster>
//{
//    private bool isCollidedWithTarget;

//    public override void EnterState()
//    {
//        owner.currentState = Monster.STATE.TRACE;
//    }

//    public override void ExitState()
//    {
//    }

//    public override void ResetState()
//    {
//        isCollidedWithTarget = false;
//    }

//    public override void Update()
//    {
//        if (Transition())
//            return;

//        if (isCollidedWithTarget)
//        {
//            owner.LookAt(owner.attackTarget.transform);
//            owner.animator.AnimIdle();
//            return;
//        }

//        owner.MoveToDestination(owner.attackTarget.transform, Time.deltaTime);
//    }

//    protected override bool Transition()
//    {
//        if (!owner.HasTarget())
//        {
//            controller.ChangeState((int)UnitOnHunt.STATE.IDLE);
//            return true;
//        }
//        else
//        {
//            isCollidedWithTarget = owner.attackTarget.stats.SizeEllipse.IsCollidedWith(owner.stats.BasicAttackEllipse);

//            if (owner.stats.AttackTimer >= owner.stats.AttackSpeed.Current && isCollidedWithTarget)
//            {
//                controller.ChangeState((int)UnitOnHunt.STATE.ATTACK);
//                return true;
//            }
//        }

//        return false;
//    }
//}