using UnityEngine;

public class TraceOnDungeon : IState<UnitOnDungeon>
{
    public override void EnterState()
    {
        controller.state = UNIT.STATE_ON_DUNGEON.TRACE;
        base.EnterState();
    }

    private void Update()
    {
        if (controller.attackTarget == null)
            return;

        controller.spriteRenderer.color = Color.yellow;

        var moveDirection = controller.transform.position - (controller.attackTarget as UnitOnDungeon).transform.position;
        transform.position -= moveDirection.normalized * Time.deltaTime * 5f;
    }
}