using System;

public interface IAttackStrategy
{
    public bool Attack(int damage, params IDamagedable[] targets);
}