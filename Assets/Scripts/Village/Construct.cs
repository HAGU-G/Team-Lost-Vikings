using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEngine;

public class Construct : MonoBehaviour
{
    public bool isSelected = false;
    public bool isRemoveTime = false;
    public bool isRoadBuild = false;
    public bool isRoadRemove = false;

    public GameObject PlaceBuilding(GameObject obj, Tile tile, GridMap gridMap)
    {
        if (!CanBuildBuilding(obj, tile, gridMap))
        {
            Debug.Log("건물을 설치할 수 없습니다.");
            isSelected = false;
            return null;
        }
            
        var objInfo = obj.GetComponent<Building>();
        //objInfo.gridMap = gridMap;
        var width = objInfo.Width;
        var height = objInfo.Length;

        objInfo.placedTiles.Clear();
        var tileId = tile.tileInfo.id;
        var indexX = tileId.x + width - 1;
        var indexY = tileId.y + height - 1;
                  
        objInfo.entranceTile = gridMap.tiles[new Vector2Int(tile.tileInfo.id.x - 1, tile.tileInfo.id.y)];
        objInfo.entranceTile.TileColorChange();
        var instancedObj = Instantiate(obj, gridMap.IndexToPos(tileId), Quaternion.identity, tile.transform);
        var pos = instancedObj.transform.position;
        pos.y = instancedObj.transform.position.y - gridMap.gridInfo.cellSize / 4f;
        instancedObj.transform.position = pos;

        var buildingComponent = instancedObj.GetComponent<Building>();
        buildingComponent.gridMap = gridMap;
        buildingComponent.placedTiles.Add(tile);

        for (int i = tileId.x; i <= indexX; ++i)
        {
            for (int j = tileId.y; j <= indexY; ++j)
            {
                var t = gridMap.tiles.GetValueOrDefault(new Vector2Int(i, j));
                t.UpdateTileInfo(TileType.OBJECT, instancedObj);

                objInfo.placedTiles.Add(t);
            }
        }
        isSelected = false;

        return instancedObj;
    }

    public GameObject PlaceRoad(GameObject road, Tile tile, GridMap gridMap)
    {
        if (!CanBuildRoad(tile, gridMap))
            return null;

        var indexX = tile.tileInfo.id.x;
        var indexY = tile.tileInfo.id.y;

        var roadObj = Instantiate(road, gridMap.IndexToPos(new Vector2Int(indexX, indexY)), Quaternion.identity, tile.transform);
        gridMap.GetTile(indexX, indexY).UpdateTileInfo(TileType.ROAD, roadObj);


        return roadObj;
    }

    public void RemoveRoad(Tile tile, GridMap gridMap)
    {
        var obj = tile.tileInfo.RoadLayer.LayerObject;
        tile.RestoreTileColor();
        tile.ResetTileInfo();
        Destroy(obj);
    }

    public GameObject RemoveBuilding(Tile tile, GridMap gridMap)
    {
        var obj = tile.tileInfo.ObjectLayer.LayerObject;
        var objInfo = obj.GetComponent<Building>();
        var width = objInfo.Width;
        var height = objInfo.Length;

        if (objInfo.placedTiles == null || !objInfo.placedTiles.Any())
            return null;

        objInfo.entranceTile.RestoreTileColor();

        var standardTile = objInfo.placedTiles.First();
        var indexX = standardTile.tileInfo.id.x + width - 1;
        var indexY = standardTile.tileInfo.id.y + height - 1;
        for (int i = standardTile.tileInfo.id.x; i <= indexX; ++i)
        {
            for (int j = standardTile.tileInfo.id.y; j <= indexY; ++j)
            {
                var t = gridMap.tiles.GetValueOrDefault(new Vector2Int(i, j));
                t?.ResetTileInfo();

            }
        }

        objInfo.placedTiles.Clear();
        Destroy(obj);
        tile.ResetTileInfo();
        isRemoveTime = false;

        return obj;
    }

    public GameObject ConstructStandardBuilding(GameObject standard, GridMap gridMap)
    {
        var buildingComponent = standard.GetComponent<Building>();

        var index = new Vector2Int(32, 31);
        var tile = gridMap.tiles[index];
        var entranceX = tile.tileInfo.id.x - 1;
        var entranceY = tile.tileInfo.id.y;
        buildingComponent.entranceTile = gridMap.tiles[new Vector2Int(entranceX, entranceY)];

        buildingComponent.placedTiles.Add(tile);
        tile.UpdateTileInfo(TileType.OBJECT, standard);

        var standardBuilding = Instantiate(standard, gridMap.IndexToPos(index), Quaternion.identity, tile.transform);
        var pos = standardBuilding.transform.position;
        pos.y = standardBuilding.transform.position.y - gridMap.gridInfo.cellSize / 4f;
        standardBuilding.transform.position = pos;

        return standardBuilding;
    }

    public bool CanBuildBuilding(GameObject obj, Tile tile, GridMap gridMap)
    {
        var building = obj.GetComponent<Building>();
        var width = building.Width;
        var length = building.Length;

        var indexX = tile.tileInfo.id.x + width - 1;
        var indexY = tile.tileInfo.id.y + length - 1;

        var entranceX = tile.tileInfo.id.x - 1;
        var entranceY = tile.tileInfo.id.y;

        if (indexX < 0 || indexY < 0
            || indexX > gridMap.gridInfo.row - 1 || indexY > gridMap.gridInfo.col - 1
            || entranceX < 0 || entranceY < 0
            || entranceX > gridMap.gridInfo.row - 1 || entranceY > gridMap.gridInfo.col - 1)
        {
            return false;
        }

        if (gridMap.GetTile(entranceX, entranceY).tileInfo.TileType == TileType.OBJECT)
            return false;

        //if (!gridMap.usingTileList.Contains(tile)
        //    || !gridMap.usingTileList.Contains(gridMap.tiles[new Vector2Int(entranceX, entranceY)]))
        //    return false;

        if(!building.CanMultiBuild)
        {
            for(int i = 0; i < gridMap.gridInfo.row -1; ++i)
            {
                for(int j = 0; j < gridMap.gridInfo.col -1; ++j)
                {
                    if(gridMap.GetTile(i,j).tileInfo.ObjectLayer.LayerObject != null 
                        && gridMap.GetTile(i,j).tileInfo.ObjectLayer.LayerObject
                        .GetComponent<Building>().StructureId
                        == obj.GetComponent<Building>().StructureId)
                    {
                        return false;
                    }
                }
            }
        }

        for (int i = tile.tileInfo.id.x; i <= indexX; ++i)
        {
            for(int j = tile.tileInfo.id.y; j <= indexY; ++j)
            {
                if (gridMap.GetTile(i,j).tileInfo.TileType == TileType.OBJECT)
                    return false;
            }
        }

        return true;
    }

    private bool CanBuildRoad(Tile tile, GridMap gridMap)
    {
        if (tile.tileInfo.TileType == TileType.OBJECT
            || tile.tileInfo.TileType == TileType.ROAD)
            return false;

        return true;
    }

}
