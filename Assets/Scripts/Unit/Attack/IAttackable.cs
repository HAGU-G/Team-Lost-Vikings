public interface IAttackable
{
    /// <returns>공격 실패: -1, 성공: 0, 이 공격으로 죽었을 경우 : 1</returns>
    public bool TryAttack();
}