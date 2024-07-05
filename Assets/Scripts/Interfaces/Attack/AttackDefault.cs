using System;

[Serializable]
public class AttackDefault : IAttackStrategy
{
    public int Attack(int damage, params IDamagedable[] targets)
    {
        int totalDamage = 0;
        foreach (var unit in targets)
        {
            var damaged = unit.TakeDamage(damage);
            totalDamage += damaged >= 0 ? damaged : 0;
        }

        return totalDamage;
    }
}