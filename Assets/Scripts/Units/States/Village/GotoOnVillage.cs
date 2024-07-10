using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GotoOnVillage : State<UnitOnVillage>
{
    public override void EnterState()
    {
        owner.currentState = UnitOnVillage.STATE.GOTO;

        var lackedParameter = owner.CheckParameter();
        switch(lackedParameter)
        {
            case UnitOnVillage.LACKING_PARAMETER.HP:
                break;
            case UnitOnVillage.LACKING_PARAMETER.STAMINA:
                owner.destination
                    = owner.villageManager.FindBuildingEntrance(STRUCTURE_TYPE.PARAMETER_RECOVERY,
                    (x) => { return x.GetComponent<ParameterRecoveryBuilding>().parameterTypes == PARAMETER_TYPES.STAMINA; });
                
                break;
            case UnitOnVillage.LACKING_PARAMETER.STRESS:
                break;
            case UnitOnVillage.LACKING_PARAMETER.NONE:
                controller.ChangeState((int)UnitOnVillage.STATE.IDLE); //부족한 파라미터가 없으면 일단 돌아다니게
                break;
        }
        
        var path = owner.FindPath(owner.villageManager.gridMap.tiles[new Vector2Int(0,0)] ,owner.destination);
        //시작 타일은 임시로 0,0에서 시작하도록 설정

        //실제 유닛 이동하는 메소드
        
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
    }

    protected override bool Transition()
    {
        
        return false;
    }
}
