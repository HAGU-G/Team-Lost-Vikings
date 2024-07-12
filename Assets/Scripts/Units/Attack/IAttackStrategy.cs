public interface IAttackStrategy
{
    /// <returns>이 공격으로 죽었을 경우 true 반환</returns>
    public bool Attack(int damage, IDamagedable target);
}