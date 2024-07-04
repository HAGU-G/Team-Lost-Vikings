using UnityEngine;

public class AttackOnDungeon : IState<UnitOnDungeon>
{
    private float attackTimer;

    public override void EnterState()
    {
        controller.state = UNIT.STATE_ON_DUNGEON.ATTACK;
        base.EnterState();
    }

    private void Update()
    {
        if (controller.attackTarget == null)
            return;

        controller.spriteRenderer.color = Color.red;

        attackTimer += Time.deltaTime;
        if (attackTimer >= controller.stats.AttackInterval)
        {
            attackTimer = 0f;
            controller.TryAttack();
        }
    }
}