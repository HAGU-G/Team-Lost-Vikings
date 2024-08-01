using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Construct
{
    public bool isSelected = false;
    public bool isRemoveTime = false;
    public bool isRoadBuild = false;
    public bool isRoadRemove = false;

    public GameObject PlaceBuilding(GameObject obj, Cell tile, GridMap gridMap)
    {
        if (!CanBuildBuilding(obj, tile, gridMap))
        {
            Debug.Log("건물을 설치할 수 없습니다.");
            isSelected = false;
            return null;
        }
            
        //var objInfo = obj.GetComponent<Building>();
        //var width = objInfo.Width;
        //var height = objInfo.Length;

        //objInfo.placedTiles.Clear();
        //var tileId = tile.tileInfo.id;
        //var indexX = tileId.x + width - 1;
        //var indexY = tileId.y + height - 1;
                  
        //objInfo.entranceTile = gridMap.tiles[new Vector2Int(tile.tileInfo.id.x - 1, tile.tileInfo.id.y)];

        var instancedObj = GameObject.Instantiate(obj, gridMap.IndexToPos(tile.tileInfo.id), Quaternion.identity, tile.transform);
        var pos = instancedObj.transform.position;
        pos.y = instancedObj.transform.position.y + gridMap.gridInfo.cellSize / (GameSetting.Instance.tileXY * 4f);
        instancedObj.transform.position = pos;

        var buildingComponent = instancedObj.GetComponent<Building>();
        buildingComponent.gridMap = gridMap;

        //TO-DO : 입구타일 4방향 수정하기
        buildingComponent.entranceTile = gridMap.tiles[new Vector2Int(tile.tileInfo.id.x - 1, tile.tileInfo.id.y)];

        SetBuildingInfo(instancedObj, tile, gridMap);
        



        isSelected = false;

        return instancedObj;
    }

    public GameObject PlaceRoad(GameObject road, Cell tile, GridMap gridMap)
    {
        if (!CanBuildRoad(tile, gridMap))
            return null;

        var indexX = tile.tileInfo.id.x;
        var indexY = tile.tileInfo.id.y;

        var roadObj = GameObject.Instantiate(road, gridMap.IndexToPos(new Vector2Int(indexX, indexY)), Quaternion.identity, tile.transform);
        gridMap.GetTile(indexX, indexY).UpdateTileInfo(TileType.ROAD, roadObj);

        //SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
        //var spriteSize = renderer.sprite.bounds.size;
        roadObj.transform.localScale = tile.transform.localScale;

        var pos = roadObj.transform.position;
        pos.y = roadObj.transform.position.y /*- (gridMap.gridInfo.cellSize / (GameSetting.Instance.tileXY * 2f))*/;
        pos.z += 1;
        roadObj.transform.position = pos;
        roadObj.transform.localScale = Vector3.one;
        return roadObj;
    }

    public void RemoveRoad(Cell tile, GridMap gridMap)
    {
        var obj = tile.tileInfo.RoadLayer.LayerObject;
        tile.RestoreTileColor();
        tile.ResetTileInfo();
        GameObject.Destroy(obj);
    }

    public GameObject RemoveBuilding(Cell tile, GridMap gridMap)
    {
        var obj = tile.tileInfo.ObjectLayer.LayerObject;
        var objInfo = obj.GetComponent<Building>();
        var width = objInfo.Width;
        var height = objInfo.Length;

        if (!CanDestroyBuilding(obj))
        {
            Debug.Log("철거할 수 없는 건물입니다.");
            return null;
        }

        if (objInfo.placedTiles == null || !objInfo.placedTiles.Any())
            return null;

        //objInfo.entranceTile.RestoreTileColor();

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
        GameObject.Destroy(obj);
        tile.ResetTileInfo();
        isRemoveTime = false;

        return obj;
    }

    //public GameObject ConstructStandardBuilding(GameObject standard, GridMap gridMap)
    //{
    //    var buildingComponent = standard.GetComponent<Building>();

    //    var index = new Vector2Int(32, 31);
    //    var tile = gridMap.tiles[index];
    //    var entranceX = tile.tileInfo.id.x - 1;
    //    var entranceY = tile.tileInfo.id.y;
    //    buildingComponent.entranceTile = gridMap.tiles[new Vector2Int(entranceX, entranceY)];

    //    buildingComponent.placedTiles.Add(tile);
    //    tile.UpdateTileInfo(TileType.OBJECT, standard);

    //    var standardBuilding = GameObject.Instantiate(standard, gridMap.IndexToPos(index), Quaternion.identity, tile.transform);
    //    var pos = standardBuilding.transform.position;
    //    pos.y = standardBuilding.transform.position.y + gridMap.gridInfo.cellSize / 4f;
    //    standardBuilding.transform.position = pos;

    //    return standardBuilding;
    //}

    public bool CanBuildBuilding(GameObject obj, Cell tile, GridMap gridMap)
    {
        var building = obj.GetComponent<Building>();
        var width = building.Width;
        var length = building.Length;

        //var indexX = tile.tileInfo.id.x + width - 1;
        //var indexY = tile.tileInfo.id.y + length - 1;

        var minX = tile.tileInfo.id.x - (width / 2);
        var minY = tile.tileInfo.id.y - (length / 2);
        var maxX = tile.tileInfo.id.x + (width / 2);
        var maxY = tile.tileInfo.id.y + (length / 2);

        //TO-DO : 입구 타일 4방향으로 수정하기
        var entranceX = tile.tileInfo.id.x - 1;
        var entranceY = tile.tileInfo.id.y;

        if (minX < 0 || minY < 0
            || maxX > gridMap.gridInfo.row - 1 || maxY > gridMap.gridInfo.col - 1
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
            //for(int i = 0; i < gridMap.gridInfo.row -1; ++i)
            //{
            //    for(int j = 0; j < gridMap.gridInfo.col -1; ++j)
            //    {
            //        if(gridMap.GetTile(i,j).tileInfo.ObjectLayer.LayerObject != null 
            //            && gridMap.GetTile(i,j).tileInfo.ObjectLayer.LayerObject
            //            .GetComponent<Building>().StructureId
            //            == obj.GetComponent<Building>().StructureId)
            //        {
            //            return false;
            //        }
            //    }
            //}
            foreach(var cell in gridMap.tiles.Values)
            {
                if(cell.tileInfo.ObjectLayer.LayerObject != null
                    && cell.tileInfo.ObjectLayer.LayerObject.GetComponent<Building>().StructureId
                        == obj.GetComponent<Building>().StructureId)
                {
                    return false;
                }
            }
        }

        for (int i = minX; i <= maxX; ++i)
        {
            for(int j = minY; j <= maxY; ++j)
            {
                if (gridMap.GetTile(i,j).tileInfo.TileType == TileType.OBJECT
                    || !gridMap.GetTile(i,j).tileInfo.MarginLayer.IsEmpty)
                    return false;
            }
        }
        return true;
    }

    public bool CanDestroyBuilding(GameObject obj)
    {
        return obj.GetComponent<Building>().CanDestroy;
    }

    private bool CanBuildRoad(Cell tile, GridMap gridMap)
    {
        if (tile.tileInfo.TileType == TileType.OBJECT
            || tile.tileInfo.TileType == TileType.ROAD)
            return false;

        return true;
    }

    public Cell GetLowestTile(GameObject obj, Cell tile, GridMap gridMap)
    {
        var building = obj.GetComponent<Building>();
        var width = building.Width;
        var length = building.Length;

        var minX = tile.tileInfo.id.x - (width / 2);
        var minY = tile.tileInfo.id.y - (length / 2);
        var maxX = tile.tileInfo.id.x + (width / 2);
        var maxY = tile.tileInfo.id.y + (length / 2);

        return gridMap.GetTile(minX, minY);
    }

    private Vector2Int GetLowestIndex(GameObject obj, Cell tile, GridMap gridMap)
    {
        var building = obj.GetComponent<Building>();
        var width = building.Width;
        var length = building.Length;

        var minX = tile.tileInfo.id.x - (width / 2);
        var minY = tile.tileInfo.id.y - (length / 2);

        return new Vector2Int(minX, minY);
    }

    private Vector2Int GetHighestIndex(GameObject obj, Cell tile, GridMap gridMap)
    {
        var building = obj.GetComponent<Building>();
        var width = building.Width;
        var length = building.Length;

        var maxX = tile.tileInfo.id.x + (width / 2);
        var maxY = tile.tileInfo.id.y + (length / 2);

        return new Vector2Int(maxX, maxY);
    }

    private void SetBuildingInfo(GameObject obj, Cell tile, GridMap gridMap)
    {
        var building = obj.GetComponent<Building>();
        var width = building.Width;
        var length = building.Length;

        var lowestIndex = GetLowestIndex(obj, tile, gridMap);
        var highestIndex = GetHighestIndex(obj, tile, gridMap);

        //기준 타일 설정
        building.standardTile = tile;
        
        //차지하는 전체 타일 설정
        building.placedTiles.Clear();
        for (int i = lowestIndex.x; i <= highestIndex.x; ++i)
        {
            for (int j = lowestIndex.y; j <= highestIndex.y; ++j)
            {
                var t = gridMap.tiles.GetValueOrDefault(new Vector2Int(i, j));

                building.placedTiles.Add(t);
            }
        }

        //여백 타일 설정
        building.marginTiles.Clear();
        for(int i = lowestIndex.x; i < lowestIndex.x + width; ++i)
        { //하단 한줄
            var t = gridMap.GetTile(i, lowestIndex.y);
            t.UpdateTileInfo(TileType.MARGIN, null);

            if(!building.marginTiles.Contains(t))
                building.marginTiles.Add(t);
        }
        
        for(int j = lowestIndex.y; j < lowestIndex.y + length; ++j)
        { //우측 한줄
            var t = gridMap.GetTile(lowestIndex.x, j);
            t.UpdateTileInfo(TileType.MARGIN, null);

            if (!building.marginTiles.Contains(t))
                building.marginTiles.Add(t);
        }

        for(int i = lowestIndex.x; i < lowestIndex.x + width; ++i)
        {//상단 한줄
            var t = gridMap.GetTile(i, highestIndex.y);
            t.UpdateTileInfo(TileType.MARGIN, null);

            if (!building.marginTiles.Contains(t))
                building.marginTiles.Add(t);
        }

        for (int j = lowestIndex.y; j < lowestIndex.y + length; ++j)
        { //좌측 한줄
            var t = gridMap.GetTile(highestIndex.x, j);
            t.UpdateTileInfo(TileType.MARGIN, null);

            if (!building.marginTiles.Contains(t))
                building.marginTiles.Add(t);
        }

        //realOccupiedTiles 설정
        foreach(var real in building.placedTiles)
        {
            if(!building.marginTiles.Contains(real))
            {
                building.realOccupiedTiles.Add(real);
                real.UpdateTileInfo(TileType.OBJECT, obj);
            }
        }
    }
}
