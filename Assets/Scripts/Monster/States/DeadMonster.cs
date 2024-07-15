using UnityEngine;

public class DeadMonster : State<Monster>
{
    public override void EnterState()
    {
        GameObject.Destroy(owner.gameObject);
    }

    public override void ExitState()
    {
    }

    public override void ResetState()
    {
    }

    public override void Update()
    {
    }

    protected override bool Transition()
    {
        return false;
    }
}