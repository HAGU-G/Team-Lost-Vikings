using System;

public interface IAttackStrategy
{
    public void Attack(int damage, params IDamagedable[] targets);
}