using System.Collections.Generic;
using UnityEngine;

public class IdleOnVillage : State<UnitOnVillage>
{
    private bool isIdle = false;
    private float timer = 0f;
    private int movableTileCount;


    public override void EnterState()
    {
        owner.currentState = UnitOnVillage.STATE.IDLE;
        isIdle = false;

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
        if(timer >= 2f)
        {
            timer = 0f;
            Debug.Log(isIdle);
        }

        //movableTileCount = 0;
        //foreach(var tile in owner.villageManager.gridMap.tiles)
        //{
        //    if(tile.Value.CanMove)
        //        ++movableTileCount;
        //}
        //if (movableTileCount == 0)
        //    return;

        if (!isIdle)
        {
            isIdle = true;
            MoveToRandomTile();
            return;
        }

    }

    protected override bool Transition()
    {
        if(owner.CheckParameter() != UnitOnVillage.LACKING_PARAMETER.NONE)
            controller.ChangeState((int)UnitOnVillage.STATE.GOTO);

        return false;
    }

    private void MoveToRandomTile()
    {
        List<Tile> movableTiles = new List<Tile>();

        foreach (var tile in owner.villageManager.gridMap.tiles.Values)
        {
            if (tile.CanMove)
            {
                movableTiles.Add(tile);
            }
        }

        if (movableTiles.Count > 0)
        {
            Tile targetTile = movableTiles[Random.Range(0, movableTiles.Count)];
            var currentPos = owner.villageManager.gridMap.PosToIndex(owner.transform.position);
            var startTile = owner.villageManager.gridMap.tiles[currentPos];

            if (startTile != targetTile)
            {
                if(owner.villageManager.gridMap.PathFinding(startTile, targetTile) != null)
                {
                    owner.unitMove.MoveTo(startTile, targetTile);
                    owner.unitMove.OnTargetTile += OnTargetTile;
                }
                else
                {
                    isIdle = false;
                    Debug.Log("이어진 길이 없습니다.");
                    return;
                }
            }
            else
            {
                isIdle = false;
                Debug.Log("경로를 다시 지정합니다.");
                return;
            }
        }
        else
        {
            isIdle = false;
            Debug.Log("이동할 수 있는 타일이 없습니다."); 
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


    public void OnTargetTile(Tile tile)
    {
        owner.unitMove.OnTargetTile -= OnTargetTile;
        isIdle = false;
    }
}