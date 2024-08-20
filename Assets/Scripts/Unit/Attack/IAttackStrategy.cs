using UnityEngine;

public interface IAttackStrategy
{
    /// <returns>이 공격으로 죽었을 경우 true 반환</returns>
    public bool Attack(IDamagedable target, int damage, bool isCritical, ATTACK_TYPE type = ATTACK_TYPE.NONE);
    public bool Attack(Vector3 targetPos, int damage, bool isCritical, ATTACK_TYPE type = ATTACK_TYPE.NONE);
}