
using UnityEngine;

public class UnitOnHunt : CombatUnit
{
    public override bool IsNeedReturn
    {
        get
        {
            return stats.HP.Ratio < GameSetting.Instance.returnHPRaito
                || stats.Stamina.Ratio < GameSetting.Instance.returnStaminaRaito
                || stats.Stress.Ratio < GameSetting.Instance.returnStressRaito;
        }
    }

    public void ResetUnit(UnitStats unitStats, HuntZone huntZone)
    {
        forceReturn = false;
        CurrentHuntZone = huntZone;
        PortalPos = CurrentHuntZone.PortalPos;
        ResetUnit(unitStats);
    }

    public override void ResetUnit(UnitStats unitStats)
    {
        base.ResetUnit(unitStats);
        stats.SetLocation(LOCATION.HUNTZONE);

        attackTarget = null;
        isTargetFixed = false;
        Enemies = CurrentHuntZone.Monsters;
        Allies = CurrentHuntZone.Units;

        FSM.ResetFSM();
    }

    public override void RemoveUnit()
    {
        base.RemoveUnit();
        GameManager.huntZoneManager.ReleaseUnit(this);
    }

    public void ReturnToVillage()
    {
        stats.SetLocation(LOCATION.NONE, LOCATION.VILLAGE);
        RemoveUnit();
    }


}