using UnityEngine;

public class ReviveOnVillage : State<UnitOnVillage>
{
    float reviveTime;
    float timer = 0f;

    public override void EnterState()
    {
        var reviveBuilding = GameManager.villageManager.GetBuilding(STRUCTURE_ID.REVIVE);
        reviveTime = reviveBuilding.GetComponent<ReviveBuilding>().reviveTime;

        owner.currentState = UnitOnVillage.STATE.REVIVE;
        var renders = owner.gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (var render in renders)
        {
            render.enabled = false;
        }
        
        

        
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

        timer += Time.deltaTime;
        if(timer >= reviveTime)
        {
            timer = 0f;
            var renders = owner.gameObject.GetComponentsInChildren<SpriteRenderer>();

            foreach (var render in renders)
            {
                render.enabled = true;
            }
            controller.ChangeState((int)UnitOnVillage.STATE.IDLE);
        }
    }

    protected override bool Transition()
    {

        return false;
    }
}