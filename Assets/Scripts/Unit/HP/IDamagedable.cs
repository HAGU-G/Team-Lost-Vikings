public interface IDamagedable
{
    ///<returns>이 공격으로 죽었을 경우 (true, 받은 데미지), 
    ///아닐경우 (false, 받은 데미지)</returns>
    public (bool, int) TakeDamage(int damage, ATTACK_TYPE type, bool isCritical);
}