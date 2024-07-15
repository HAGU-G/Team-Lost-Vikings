using UnityEngine;

public class Monster : MonoBehaviour, IDamagedable, ISubject<Monster>
{

    public MonsterStats stats = new();

    /// <summary>
    /// base.Init()가 최상단에 있어야함.
    /// </summary>
    protected virtual void Init()
    {
        //TESTCODE
        //TODO 스탯 할당, 스킬 할당
        stats.Init();
    }

    /// <summary>
    /// base.ResetUnit()이 최상단에 있어야함.
    /// </summary>
    protected virtual void ResetMonster()
    {
        ResetEvents();
        stats.ResetStats();
    }

    /// <summary>
    /// base.ResetEvents()가 최상단에 있어야함.
    /// </summary>
    protected virtual void ResetEvents() { }

    public void Subscribe(IObserver<Monster> observer)
    {
        throw new System.NotImplementedException();
    }

    public bool TakeDamage(int damage)
    {
        throw new System.NotImplementedException();
    }

    public void UnSubscrive(IObserver<Monster> observer)
    {
        throw new System.NotImplementedException();
    }
}
