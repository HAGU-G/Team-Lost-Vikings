﻿using System.Collections;
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
                SetDestination(PARAMETER_TYPES.HP);
                break;
            case UnitOnVillage.LACKING_PARAMETER.STAMINA:
                SetDestination(PARAMETER_TYPES.STAMINA);
                break;
            case UnitOnVillage.LACKING_PARAMETER.STRESS:
                SetDestination(PARAMETER_TYPES.STRESS);
                break;
            case UnitOnVillage.LACKING_PARAMETER.NONE:
                controller.ChangeState((int)UnitOnVillage.STATE.IDLE);
                //부족한 파라미터가 없으면 일단 돌아다니게
                break;
        }
        
        if(owner.destination != null)
        {
            var tileId = owner.villageManager.gridMap.PosToIndex(owner.gameObject.transform.position);
            var startTile = owner.villageManager.gridMap
                .tiles[new Vector2Int(tileId.x, tileId.y)];
            var path = owner.FindPath(startTile, owner.destinationTile);
            if (path != null || startTile == owner.destinationTile)
            {
                owner.unitMove.OnEntranceTile += OnEntranceTile;
                owner.unitMove.MoveTo(startTile, owner.destinationTile);
            }
            else
                Debug.Log("가야할 건물의 타입이 없습니다.");
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

    private void SetDestination(PARAMETER_TYPES parameterType)
    {
        if (owner.villageManager.FindBuilding(STRUCTURE_TYPE.PARAMETER_RECOVERY,
                   (x) => 
                   {
                       if (x.GetComponent<ParameterRecoveryBuilding>() != null)
                       {
                           return x.GetComponent<ParameterRecoveryBuilding>().parameterType == parameterType;
                       }
                       return false;
                   }))
        {
            owner.destination
            = owner.villageManager.FindBuildingEntrance(STRUCTURE_TYPE.PARAMETER_RECOVERY,
            (x) =>
            {
                {
                    if (x.GetComponent<ParameterRecoveryBuilding>() != null)
                    {
                        return x.GetComponent<ParameterRecoveryBuilding>().parameterType == parameterType;
                    }
                    return false;
                }
            });
            owner.destinationTile = owner.destination.GetComponent<Building>().entranceTile;
        }
        
    }

    private void OnEntranceTile(Tile tile)
    {
        
        if (tile == owner.destinationTile)
        {
            owner.unitMove.OnEntranceTile -= OnEntranceTile;
            Debug.Log("ChangeState");
            controller.ChangeState((int)UnitOnVillage.STATE.INTERACT);
        }
    }
}
