using System.Collections.Generic;
using UnityEngine;

public class GotoOnVillage : State<UnitOnVillage>
{
    public override void EnterState()
    {
        owner.currentState = UnitOnVillage.STATE.GOTO;

        if (owner.forceDestination != null)
        {
            var tileId = owner.villageManager.gridMap.PosToIndex(owner.gameObject.transform.position);
            var startTile = owner.villageManager.gridMap
                .tiles[new Vector2Int(tileId.x, tileId.y)];
            var forceTiles = owner.forceDestination.GetComponent<Building>().entranceTiles;

            List<Cell> path = new();
            Cell cell = null;
            (path, cell) = owner.FindShortestPath(startTile, forceTiles);

            //var path = owner.FindPath(startTile, forceTile);
            if (path != null || startTile == cell)
            {
                owner.unitMove.OnTargetTile += OnEntranceTile;
                owner.unitMove.MoveTo(startTile, cell);
                return;
            }
            else
            {
                Debug.Log("가야할 건물의 타입이 없습니다.");
            }
        }

        var lackedParameter = owner.CheckParameter();

        switch (lackedParameter)
        {
            case UnitOnVillage.LACKING_PARAMETER.HP:
                SetDestination(PARAMETER_TYPE.HP);
                break;
            case UnitOnVillage.LACKING_PARAMETER.STAMINA:
                SetDestination(PARAMETER_TYPE.STAMINA);
                break;
            case UnitOnVillage.LACKING_PARAMETER.STRESS:
                SetDestination(PARAMETER_TYPE.MENTAL);
                break;
            case UnitOnVillage.LACKING_PARAMETER.NONE:
                if (GameManager.huntZoneManager.HuntZones.ContainsKey(owner.stats.HuntZoneNum))
                {
                    foreach (var constructed in GameManager.villageManager.constructedBuildings)
                    {
                        var building = constructed.GetComponent<Building>();
                        if (building.StructureType == STRUCTURE_TYPE.PORTAL)
                        {
                            owner.destination = building.gameObject;
                            break;
                        }
                    }
                    owner.destinationTiles = owner.destination.GetComponent<Building>().entranceTiles;
                }
                //controller.ChangeState((int)UnitOnVillage.STATE.IDLE);
                //부족한 파라미터가 없으면 일단 돌아다니게 -> 사냥터로
                break;
        }

        if (owner.destination != null)
        {
            var tileId = owner.villageManager.gridMap.PosToIndex(owner.gameObject.transform.position);
            var startTile = owner.villageManager.gridMap
                .tiles[new Vector2Int(tileId.x, tileId.y)];

            List<Cell> path = new();
            Cell cell = null;
            (path, cell) = owner.FindShortestPath(startTile, owner.destinationTiles);

            //var path = owner.FindPath(startTile, owner.destinationTile);
            if (path != null || startTile == cell)
            {
                owner.destinationTile = cell;
                owner.unitMove.OnTargetTile += OnEntranceTile;
                owner.unitMove.MoveTo(startTile, owner.destinationTile);
            }
            else
                Debug.Log("가야할 건물의 타입이 없습니다.");
        }
    }

    public override void ExitState()
    {
        owner.unitMove.OnTargetTile -= OnEntranceTile;
        ParameterRecoveryBuilding building = null;
        if(owner.forceDestination != null)
            building = owner.forceDestination.GetComponent<ParameterRecoveryBuilding>();

        if(owner.destination != null)
            building = owner.destination.GetComponent<ParameterRecoveryBuilding>();

        if (building != null)
        {
            building.RemoveMovingUnit(owner);
        }
    }

    public override void ResetState()
    {
        owner.destination = null;
        owner.destinationTile = null;
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

    private void SetDestination(PARAMETER_TYPE parameterType)
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
            owner.destinationTiles = owner.destination.GetComponent<Building>().entranceTiles;

            var start = owner.villageManager.gridMap.PosToIndex(owner.transform.position);
            var startTile = owner.villageManager.GetTile(start.x, start.y, owner.villageManager.gridMap);
            
            var (path, cell) = owner.FindShortestPath(startTile, owner.destinationTiles);

            owner.destinationTile = cell;
        }
        else
        {
            //해당 건물 없을 때 Idle 상태
            controller.ChangeState((int)UnitOnVillage.STATE.IDLE);
            owner.isBuildingExist = false;
        }

    }

    private void OnEntranceTile(Cell tile)
    {
        if (owner.forceDestination != null)
        {
            owner.unitMove.OnTargetTile -= OnEntranceTile;
            controller.ChangeState((int)UnitOnVillage.STATE.IDLE);
            owner.forceDestination = null;
            return;
        }

        if (tile == owner.destinationTile)
        {
            owner.unitMove.OnTargetTile -= OnEntranceTile;
            controller.ChangeState((int)UnitOnVillage.STATE.INTERACT);
        }
    }
}
