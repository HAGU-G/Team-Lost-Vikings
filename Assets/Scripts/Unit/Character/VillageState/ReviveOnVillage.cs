using UnityEngine;

public class ReviveOnVillage : State<UnitOnVillage>
{
    private float reviveTime;
    private float timer = 0f;
    private ReviveBuilding reviveBuilding;
    private bool isReviving = false;

    public override void EnterState()
    {
        var obj = GameManager.villageManager.GetBuilding(STRUCTURE_ID.REVIVE);
        reviveBuilding = obj.GetComponent<ReviveBuilding>();
        reviveTime = reviveBuilding.reviveTime;
        reviveBuilding.revivingUnits.Add(owner);
        isReviving = true;
        owner.currentState = UnitOnVillage.STATE.REVIVE;
        var renders = owner.gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (var render in renders)
        {
            render.enabled = false;
        }
        GameManager.uiManager.windows[WINDOW_NAME.REVIVE_POPUP].GetComponent<UIReviveBuilding>().SetRevivingList();
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
        if (isReviving)
        {
            GameManager.uiManager.windows[WINDOW_NAME.REVIVE_POPUP].GetComponent<UIReviveBuilding>().SetProgressBar(timer, reviveTime);
        }
        
        if (timer >= reviveTime)
        {
            timer = 0f;
            isReviving = false;
            var renders = owner.gameObject.GetComponentsInChildren<SpriteRenderer>();

            foreach (var render in renders)
            {
                render.enabled = true;
            }
            reviveBuilding.revivingUnits.Remove(owner);
            GameManager.uiManager.windows[WINDOW_NAME.REVIVE_POPUP].GetComponent<UIReviveBuilding>().SetRevivingList();
            controller.ChangeState((int)UnitOnVillage.STATE.IDLE);
        }
    }

    protected override bool Transition()
    {

        return false;
    }
}