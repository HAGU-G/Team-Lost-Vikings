public interface IDamagedable
{
    ///<returns>이 공격으로 죽었을 경우 true 반환</returns>
    public bool TakeDamage(int damage, ATTACK_TYPE type);
}