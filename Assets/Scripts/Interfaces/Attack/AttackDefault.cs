using System;

[Serializable]
public class AttackDefault : IAttackStrategy
{
    public void Attack(int damage, params IDamagedable[] targets)
    {
        foreach (var unit in targets)
        {
            unit.TakeDamage(damage);
        }
    }
}