﻿using System.Collections.Generic;
using UnityEngine;

public class IdleOnVillage : State<UnitOnVillage>
{
    private bool isIdle = false;
    private int movableTileCount;


    public override void EnterState()
    {
        owner.currentState = UnitOnVillage.STATE.IDLE;
        isIdle = false;
        owner.isRecoveryQuited = false;
    }

    public override void ExitState()
    {
        isIdle = false;
    }

    public override void ResetState()
    {
        isIdle = false;
    }

    public override void Update()
    {
        if (Transition())
            return;

        if (!isIdle)
        {
            isIdle = true;
            MoveToRandomTile();
            return;
        }
    }

    protected override bool Transition()
    {
        if(owner.CheckParameter() != UnitOnVillage.LACKING_PARAMETER.NONE
            ||GameManager.huntZoneManager.HuntZones.ContainsKey(owner.stats.HuntZoneNum)
            //|| owner.forceDestination != null
            //|| owner.destination != null
            || owner.isRecoveryQuited == true)
        {
            if (owner.CheckParameter() != UnitOnVillage.LACKING_PARAMETER.NONE
                && !owner.villageManager.FindBuilding(STRUCTURE_TYPE.PARAMETER_RECOVERY,
                   (x) =>
                   {
                       if (x.GetComponent<ParameterRecoveryBuilding>() != null)
                       {
                           return (int)x.GetComponent<ParameterRecoveryBuilding>().parameterType == (int)owner.CheckParameter();
                       }

                       return false;
                   }))
            {
                return false;
            }

            controller.ChangeState((int)UnitOnVillage.STATE.GOTO);
            return true;
        }
            return false;
    }

    private void MoveToRandomTile()
    {
        List<Cell> movableTiles = new List<Cell>();

        foreach (var tile in owner.villageManager.gridMap.tiles.Values)
        {
            if (tile.CanMove)
            {
                movableTiles.Add(tile);
            }
        }

        if (movableTiles.Count > 0)
        {
            Cell targetTile = movableTiles[Random.Range(0, movableTiles.Count)];
            var currentIndex = owner.villageManager.gridMap.PosToIndex(owner.transform.position);
            var startTile = owner.villageManager.gridMap.tiles[currentIndex];

            if (startTile != targetTile)
            {
                if(owner.villageManager.gridMap.PathFinding(startTile, targetTile) != null)
                {
                    var move = owner.unitMove.MoveTo(startTile, targetTile);
                    if(!move)
                    {
                        isIdle = false;
                        return;
                    }
                    owner.unitMove.OnTargetTile += OnTargetTile;
                }
                else
                {
                    isIdle = false;
                    //Debug.Log("이어진 길이 없습니다.");
                    return;
                }
            }
            else
            {
                isIdle = false;
                //Debug.Log("경로를 다시 지정합니다.");
                return;
            }
        }
        else
        {
            isIdle = false;
            //Debug.Log("이동할 수 있는 타일이 없습니다."); 
            return;
        }

        //이어지지 않은 타일들만 존재하는 경우
        //foreach (var tile in movableTiles)
        //{
        //    var currentTile = owner.villageManager.gridMap.tiles[owner.villageManager.gridMap.PosToIndex(owner.transform.position)];
        //    if (owner.villageManager.gridMap.PathFinding(currentTile, tile) == null)
        //    {
        //        continue;
        //    }
        //    else
        //    {
        //        isIdle = false;
        //        return;
        //    }    
        //}

    }


    public void OnTargetTile(Cell tile)
    {
        owner.unitMove.OnTargetTile -= OnTargetTile;
        isIdle = false;
    }
}