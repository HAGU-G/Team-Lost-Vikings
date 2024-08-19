using System;
using UnityEngine;

[Serializable]
public class AttackDefault : IAttackStrategy
{
    public bool Attack(IDamagedable target, int damage, bool isCritical, ATTACK_TYPE type = ATTACK_TYPE.NONE)
    {
        if (target != null)
            return target.TakeDamage(damage, type, isCritical).Item1;
        else
            return false;
    }

    public bool Attack(Vector3 targetPos, int damage, bool isCritical, ATTACK_TYPE type = ATTACK_TYPE.NONE)
    {
        return false;
    }
}