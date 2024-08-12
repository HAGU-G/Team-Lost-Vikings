using System;

[Serializable]
public class AttackDefault : IAttackStrategy
{
    public bool Attack(IDamagedable target, int damage, ATTACK_TYPE type = ATTACK_TYPE.NONE)
    {
        return target.TakeDamage(damage, type).Item1;
    }
}