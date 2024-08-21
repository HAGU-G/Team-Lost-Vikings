using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReviveOnVillage : State<UnitOnVillage>
{
    private float reviveTime;
    private ReviveBuilding reviveBuilding;
    private bool isReviving = false;
    private List<SpriteRenderer> renderers = new();

    public override void EnterState()
    {
        var obj = GameManager.villageManager.GetBuilding(STRUCTURE_ID.REVIVE);
        reviveBuilding = obj.GetComponent<ReviveBuilding>();
        reviveTime = reviveBuilding.reviveTime;
        reviveBuilding.revivingUnits.Add(owner);
        isReviving = true;
        owner.currentState = UnitOnVillage.STATE.REVIVE;
        renderers = owner.gameObject.GetComponentsInChildren<SpriteRenderer>().ToList();
        foreach (var render in renderers)
        {
            render.enabled = false;
        }
        GameManager.uiManager.windows[WINDOW_NAME.REVIVE_POPUP].GetComponent<UIReviveBuilding>().SetRevivingList();
    }

    public override void ExitState()
    {
        reviveBuilding?.revivingUnits.Remove(owner);
        isReviving = false;
        GameManager.uiManager.windows[WINDOW_NAME.REVIVE_POPUP].GetComponent<UIReviveBuilding>().SetRevivingList();
        foreach (var render in renderers)
        {
            render.enabled = true;
        }
    }

    public override void ResetState()
    {
        reviveBuilding?.revivingUnits.Remove(owner);
        isReviving = false;
        GameManager.uiManager.windows[WINDOW_NAME.REVIVE_POPUP].GetComponent<UIReviveBuilding>().SetRevivingList();
        foreach (var render in renderers)
        {
            render.enabled = true;
        }
    }

    public override void Update()
    {
        if (renderers == null || renderers.Count <= 1)
        {
            renderers = owner.gameObject.GetComponentsInChildren<SpriteRenderer>().ToList();
            foreach (var render in renderers)
            {
                render.enabled = false;
            }
        }

        if (Transition())
            return;

        owner.stats.reviveTimer += Time.deltaTime;
        if (isReviving)
        {
            GameManager.uiManager.windows[WINDOW_NAME.REVIVE_POPUP].GetComponent<UIReviveBuilding>().SetProgressBar(owner.stats.reviveTimer, reviveTime);
        }
        
        if (owner.stats.reviveTimer >= reviveTime)
        {
            owner.stats.isDead = false;
            owner.stats.reviveTimer = 0f;
            isReviving = false;

            foreach (var render in renderers)
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