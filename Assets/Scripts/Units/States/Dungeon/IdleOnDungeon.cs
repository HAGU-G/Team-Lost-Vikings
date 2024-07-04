using UnityEngine;

public class IdleOnDungeon : IState<UnitOnDungeon>
{
    public override void EnterState()
    {
        controller.state = UNIT.STATE_ON_DUNGEON.IDLE;
        base.EnterState();
    }

    private void Update()
    {
        controller.spriteRenderer.color = Color.white;
    }
}