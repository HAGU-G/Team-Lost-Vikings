public interface IDamagedable
{
    ///<returns>받은 피해, 죽은 경우 음수로 반환</returns>
    public int TakeDamage(int damage);
}