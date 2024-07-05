public interface IAttackStrategy
{
    /// <returns>입힌 피해</returns>
    public int Attack(int damage, params IDamagedable[] targets);
}