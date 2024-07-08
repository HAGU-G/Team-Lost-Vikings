using UnityEngine;

public class AttackOnDungeon : State<UnitOnDungeon>
{
    private float attackTimer;

    public override void EnterState()
    {
        owner.currentState = UnitOnDungeon.STATE.ATTACK;
        owner.spriteRenderer.color = Color.red;
    }

    public override void ExitState()
    {
    }

    public override void ResetState()
    {
    }

    public override void Update()
    {
        if (Transition())
            return;

        attackTimer += Time.deltaTime;
        if (attackTimer >= owner.stats.CurrentStats.AttackSpeed)
        {
            attackTimer = 0f;
            owner.TryAttack();
        }
    }

    protected override bool Transition()
    {
        if (owner.attackTarget == null)
        {
            controller.ChangeState((int)UnitOnDungeon.STATE.IDLE);
            return true;
        }

        return false;
    }
}