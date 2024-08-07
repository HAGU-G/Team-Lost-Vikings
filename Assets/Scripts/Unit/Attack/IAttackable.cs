public interface IAttackable
{
    /// <returns>공격 성공 시 true 반환</returns>
    public bool TryAttack();
    public bool HasTarget();
}