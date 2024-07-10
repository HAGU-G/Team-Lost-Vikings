using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
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
                if(owner.villageManager.FindBuilding(STRUCTURE_TYPE.PARAMETER_RECOVERY,
                    (x) => { return x.GetComponent<ParameterRecoveryBuilding>().parameterTypes == PARAMETER_TYPES.STAMINA; }))
                {
                    owner.destination
                    = owner.villageManager.FindBuildingEntrance(STRUCTURE_TYPE.PARAMETER_RECOVERY,
                    (x) => { return x.GetComponent<ParameterRecoveryBuilding>().parameterTypes == PARAMETER_TYPES.STAMINA; });
                }
                else
                {
                    Debug.Log("가야할 건물의 타입이 없습니다.");
                }
                break;
            case UnitOnVillage.LACKING_PARAMETER.STRESS:
                break;
            case UnitOnVillage.LACKING_PARAMETER.NONE:
                controller.ChangeState((int)UnitOnVillage.STATE.IDLE); //부족한 파라미터가 없으면 일단 돌아다니게
                break;
        }
        //시작 타일은 임시로 0,0에서 시작하도록 설정
        
        if(owner.destination != null)
        {
            var startTile = owner.villageManager.gridMap.tiles[new Vector2Int(0, 0)];
            var path = owner.FindPath(startTile, owner.destination);
            if (path != null)
                owner.unitMove.MoveTo(startTile, owner.destination);
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

        //사용하려던 건물이 없다가 생겼을 때 바로 찾아가야함
    }

    protected override bool Transition()
    {
        
        return false;
    }
}
