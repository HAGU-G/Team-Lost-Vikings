using System;

[Serializable]
public class AttackDefault : IAttackStrategy
{
    public bool Attack(IDamagedable target, int damage, ATTACK_TYPE type = ATTACK_TYPE.NONE)
    {
        var damaged = target.TakeDamage(damage, type);
        return true;
    }
}