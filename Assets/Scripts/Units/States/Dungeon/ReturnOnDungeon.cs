using UnityEngine;

public class ReturnOnDungeon : State<UnitOnDungeon>
{
    private float recoveryTimer;

    public override void EnterState()
    {
        owner.attackTarget = null;
        owner.currentState = UnitOnDungeon.STATE.RETURN;
        owner.spriteRenderer.color = Color.black;
    }

    public override void ExitState()
    {
    }

    public override void ResetState()
    {
    }

    public override void Update()
    {
        recoveryTimer += Time.deltaTime;
        if (recoveryTimer >= 5f)
        {
            recoveryTimer = 0f;
            owner.stats.ResetStats();
            Transition();
        }
    }

    protected override bool Transition()
    {
        controller.ChangeState((int)UnitOnDungeon.STATE.IDLE);
        return true;
    }
}