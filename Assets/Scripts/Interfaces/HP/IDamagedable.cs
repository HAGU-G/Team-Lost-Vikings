public interface IDamagedable
{
    ///<returns>받은 피해, 죽은 경우: -1</returns>
    public int TakeDamage(int damage);
}