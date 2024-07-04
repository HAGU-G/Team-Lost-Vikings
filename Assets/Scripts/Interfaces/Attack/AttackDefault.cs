using System;

[Serializable]
public class AttackDefault : IAttackStrategy
{
    public bool Attack(int damage, params IDamagedable[] targets)
    {
        bool result = false;
        foreach (var unit in targets)
        {
            result = unit.TakeDamage(damage);
        }

        return result;
    }
}