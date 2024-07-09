using System;

[Serializable]
public class AttackDefault : IAttackStrategy
{
    public bool Attack(int damage, IDamagedable target)
    {
        var damaged = target.TakeDamage(damage);
        return true;
    }
}