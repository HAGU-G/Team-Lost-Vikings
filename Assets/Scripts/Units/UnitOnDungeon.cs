using System;
using UnityEngine;
using UnityEngine.UI;

public class UnitOnDungeon : Unit, IDamagedable
{
    private IAttackStrategy attackBehaviour = new AttackDefault();
    private IDamagedable attackTarget;

    public event Action OnDamaged;

    protected override void Init()
    {
        base.Init(); 

        //TESTCODE
        OnDamaged += () => { Debug.Log(stats.CurrentHP); };
    }

    protected void Awake()
    {
        Init();
    }




    public void TakeDamage(int damage)
    {
        stats.CurrentHP -= damage;
        OnDamaged?.Invoke();
    }

    protected void TryAttack()
    {
        attackBehaviour.Attack(stats.AttackDamage, attackTarget);
    }


}
