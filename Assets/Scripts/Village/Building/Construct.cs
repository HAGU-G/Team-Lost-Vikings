using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Construct
{
    public bool isSelected = false;
    public bool isRemoveTime = false;
    public bool isRoadBuild = false;
    public bool isRoadRemove = false;

    List<Cell> buildingCells = new(); 
    public List<Cell> previousHighlightedCells = new List<Cell>();
    public Cell selectedCell;

    public GameObject PlaceBuilding(GameObject obj, Cell tile, GridMap gridMap)
    {
        if (!CanBuildBuilding(obj, tile, gridMap))
        {
            Debug.Log("건물을 설치할 수 없습니다.");
            isSelected = false;
            return null;
        }

        var instancedObj = GameObject.Instantiate(obj, gridMap.IndexToPos(tile.tileInfo.id), Quaternion.identity, tile.transform);
        var pos = instancedObj.transform.position;
        pos.y = instancedObj.transform.position.y + gridMap.gridInfo.cellSize / (GameSetting.Instance.tileXY * 4f);
        instancedObj.transform.position = pos;

        var buildingComponent = instancedObj.GetComponent<Building>();
        buildingComponent.gridMap = gridMap;
        SetBuildingInfo(instancedObj, tile, gridMap);

        //설치하며 이전 업그레이드 단계가 있었는지 검사 후 적용
        if(gridMap == GameManager.villageManager.gridMap
            && GameManager.playerManager.buildingUpgradeGrades.TryGetValue(buildingComponent.StructureId, out var value)
            && instancedObj.GetComponent<BuildingUpgrade>() != null)
        {
            instancedObj.GetComponent<BuildingUpgrade>().Upgrade(true);
        }
        
        isSelected = false;
        if(gridMap == GameManager.villageManager.gridMap)
            GameManager.villageManager.constructedBuildings.Add(instancedObj);
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

    public bool RemoveBuilding(Building building, GridMap gridMap)
    {
        if (!CanDestroyBuilding(building.gameObject))
            return false;

        foreach(var tile in gridMap.tiles.Values)
        {
            if(!tile.tileInfo.ObjectLayer.IsEmpty)
            {
                var b = tile.tileInfo.ObjectLayer.LayerObject.GetComponent<Building>();
                if (b == building)
                {
                    

                    var upgrade = tile.tileInfo.ObjectLayer.LayerObject.GetComponent<BuildingUpgrade>();
                    if (upgrade != null)
                    {
                        if (GameManager.playerManager.buildingUpgradeGrades.TryGetValue(b.StructureId, out int value))
                        {
                            value = upgrade.currentGrade;
                        }
                        else
                        {
                            GameManager.playerManager.buildingUpgradeGrades.Add(b.StructureId, upgrade.currentGrade);
                        }
                    }

                    foreach (var t in building.placedTiles)
                        t.ResetTileInfo();

                    GameObject.Destroy(building.gameObject);
                    GameManager.villageManager.constructedBuildings.Remove(building.gameObject);

                    
                    return true;
                }
                else
                    continue;
            }
        }
        return false;
    }

    public bool ForceRemovingBuilding(Building building, GridMap gridMap)
    {
        foreach (var tile in gridMap.tiles.Values)
        {
            if (!tile.tileInfo.ObjectLayer.IsEmpty)
            {
                var b = tile.tileInfo.ObjectLayer.LayerObject.GetComponent<Building>();
                if (b == building)
                {
                    var upgrade = tile.tileInfo.ObjectLayer.LayerObject.GetComponent<BuildingUpgrade>();
                    if (upgrade != null)
                    {
                        if (GameManager.playerManager.buildingUpgradeGrades.TryGetValue(b.StructureId, out int value))
                        {
                            value = upgrade.currentGrade;
                        }
                        else
                        {
                            GameManager.playerManager.buildingUpgradeGrades.Add(b.StructureId, upgrade.currentGrade);
                        }
                    }

                    foreach (var t in building.placedTiles)
                        t.ResetTileInfo();

                    GameObject.Destroy(building.gameObject);
                    GameManager.villageManager.constructedBuildings.Remove(building.gameObject);
                    return true;
                }
                else
                    continue;
            }
        }
        return false;
    }

    //public GameObject ReplaceBuilding(GameObject obj, Cell tile, GridMap gridMap) 
    //{
    //    if (!CanReplaceBuilding(obj, tile, gridMap))
    //        return null;

    //    var building = obj.GetComponent<Building>();
    //    foreach(var t in building.placedTiles)
    //    {
    //        t.ResetTileInfo();
    //    }

    //    var pos = gridMap.IndexToPos(tile.tileInfo.id);
    //    obj.transform.SetParent(tile.gameObject.transform);
    //    obj.transform.position = pos;
    //    SetBuildingInfo(obj, tile, gridMap);

    //    return obj;
    //}

    private bool CanReplaceBuilding(GameObject obj, Cell tile, GridMap gridMap)
    {
        var building = obj.GetComponent<Building>();
        var width = building.Width;
        var length = building.Length;

        var minX = tile.tileInfo.id.x - (width / 2);
        var minY = tile.tileInfo.id.y - (length / 2);
        var maxX = tile.tileInfo.id.x + (width / 2);
        var maxY = tile.tileInfo.id.y + (length / 2);

        if (minX < 0 || minY < 0
            || maxX > gridMap.gridInfo.row - 1 || maxY > gridMap.gridInfo.col - 1)
        {
            return false;
        }

        for (int i = minX; i <= maxX; ++i)
        {
            for (int j = minY; j <= maxY; ++j)
            {
                var t = gridMap.GetTile(i, j);
                if (building.placedTiles.Contains(t))
                    continue;

                if (t.tileInfo.TileType == TileType.OBJECT
                    || !t.tileInfo.MarginLayer.IsEmpty)
                    return false;

                if (gridMap == GameManager.villageManager.gridMap)
                {
                    if (!gridMap.usingTileList.Contains(t))
                        return false;
                }
            }
        }
        return true;
    }

    //public bool CanReplaceBuilding(int width, int length, Cell tile, GridMap gridMap)
    //{

    //    var minX = tile.tileInfo.id.x - (width / 2);
    //    var minY = tile.tileInfo.id.y - (length / 2);
    //    var maxX = tile.tileInfo.id.x + (width / 2);
    //    var maxY = tile.tileInfo.id.y + (length / 2);

    //    if (minX < 0 || minY < 0
    //        || maxX > gridMap.gridInfo.row - 1 || maxY > gridMap.gridInfo.col - 1)
    //    {
    //        return false;
    //    }

    //    for (int i = minX; i <= maxX; ++i)
    //    {
    //        for (int j = minY; j <= maxY; ++j)
    //        {
    //            if (building.placedTiles.Contains(gridMap.GetTile(i, j)))
    //                continue;

    //            if (gridMap.GetTile(i, j).tileInfo.TileType == TileType.OBJECT
    //                || !gridMap.GetTile(i, j).tileInfo.MarginLayer.IsEmpty)
    //                return false;
    //        }
    //    }
    //    return true;
    //}


    public bool CanBuildBuilding(GameObject obj, Cell tile, GridMap gridMap)
    {
        var building = obj.GetComponent<Building>();
        var width = building.Width;
        var length = building.Length;

        var minX = tile.tileInfo.id.x - (width / 2);
        var minY = tile.tileInfo.id.y - (length / 2);
        var maxX = tile.tileInfo.id.x + (width / 2);
        var maxY = tile.tileInfo.id.y + (length / 2);

        if (minX < 0 || minY < 0
            || maxX > gridMap.gridInfo.row - 1 || maxY > gridMap.gridInfo.col - 1)
        {
            return false;
        }

        if (!building.CanMultiBuild)
        {
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
                var t = gridMap.GetTile(i, j);
                if (t.tileInfo.TileType == TileType.OBJECT
                    || !t.tileInfo.MarginLayer.IsEmpty)
                    return false;

                if(GameManager.IsReady && gridMap == GameManager.villageManager.gridMap)
                {
                    if (!gridMap.usingTileList.Contains(t))
                        return false;
                }
            }
        }
        return true;
    }

    public bool CanBuildBuilding(int width, int length, Cell tile, GridMap gridMap)
    {
        var minX = tile.tileInfo.id.x - (width / 2);
        var minY = tile.tileInfo.id.y - (length / 2);
        var maxX = tile.tileInfo.id.x + (width / 2);
        var maxY = tile.tileInfo.id.y + (length / 2);

        if (minX < 0 || minY < 0
            || maxX > gridMap.gridInfo.row - 1 || maxY > gridMap.gridInfo.col - 1)
        {
            return false;
        }

        for (int i = minX; i <= maxX; ++i)
        {
            for (int j = minY; j <= maxY; ++j)
            {
                var t = gridMap.GetTile(i, j);
                if (t.tileInfo.TileType == TileType.OBJECT
                    || !t.tileInfo.MarginLayer.IsEmpty)
                    return false;

                if (gridMap == GameManager.villageManager.gridMap)
                {
                    if (!gridMap.usingTileList.Contains(t))
                        return false;
                }
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
                t.UpdateTileInfo(TileType.OBJECT, obj);
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

        //입구 타일 설정
        building.entranceTiles.Add(gridMap.GetTile(lowestIndex.x, tile.tileInfo.id.y));
        building.entranceTiles.Add(gridMap.GetTile(tile.tileInfo.id.x, highestIndex.y));
        building.entranceTiles.Add(gridMap.GetTile(highestIndex.x, tile.tileInfo.id.y));
        building.entranceTiles.Add(gridMap.GetTile(tile.tileInfo.id.x, lowestIndex.y));
    }

    public void MakeBuildingGrid()
    {
        var buildingData = GameManager.uiManager.currentBuildingData;
        var width = buildingData.Width;
        var length = buildingData.Length;
        var gridMap = GameManager.villageManager.gridMap;

        Vector3 centerPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));
        Vector2Int centerIndex = gridMap.PosToIndex(centerPos);

        CreateBuildingGrid(centerIndex, width, length);
    }

    public void UpdateHighlightedCells(Vector2Int centerIndex)
    {
        if(previousHighlightedCells.Count != 0)
        {
            foreach (var cell in previousHighlightedCells)
            {
                cell.RestoreTileColor();
            }
            previousHighlightedCells.Clear();
        }
        
        CreateBuildingGrid(centerIndex, GameManager.uiManager.currentBuildingData.Width, GameManager.uiManager.currentBuildingData.Length);
    }

    private void CreateBuildingGrid(Vector2Int centerIndex, int width, int length)
    {
        var gridMap = GameManager.villageManager.gridMap;

        for (int x = -width / 2; x <= width / 2; x++)
        {
            for (int y = -length / 2; y <= length / 2; y++)
            {
                Vector2Int cellIndex = new Vector2Int(centerIndex.x + x, centerIndex.y + y);
                Cell cell = gridMap.GetTile(cellIndex.x, cellIndex.y);
                if (cell != null)
                {
                    previousHighlightedCells.Add(cell);
                }
            }
        }

        

        selectedCell = gridMap.GetTile(centerIndex.x, centerIndex.y);
        var constructComplete = GameManager.uiManager.uiDevelop.constructComplete;
        constructComplete.SetActive(true);
        var constructCompo = constructComplete.GetComponent<BuildComplete>();
        constructComplete.gameObject.transform.position = gridMap.IndexToPos(centerIndex);

        var constructMode = GameManager.uiManager.windows[WINDOW_NAME.CONSTRUCT_MODE] as UIConstructMode;
        if(constructMode.isConstructing)
        {
            foreach (var tile in previousHighlightedCells)
            {
                if (!tile.tileInfo.MarginLayer.IsEmpty || !tile.tileInfo.ObjectLayer.IsEmpty)
                    tile.TileColorChange(false);
                else
                    tile.TileColorChange(true);
            }
            if (GameManager.villageManager.construct.CanBuildBuilding(width, length, selectedCell, gridMap))
            { constructCompo.yesButton.interactable = true; }
            else
            { constructCompo.yesButton.interactable = false; }
        }
        else if(constructMode.IsReplacing)
        {
            foreach (var tile in previousHighlightedCells)
            {
                if(GameManager.uiManager.currentNormalBuidling.placedTiles.Contains(tile))
                {
                    tile.TileColorChange(true);
                    continue;
                }

                if (!tile.tileInfo.MarginLayer.IsEmpty || !tile.tileInfo.ObjectLayer.IsEmpty)
                    tile.TileColorChange(false);
                else
                    tile.TileColorChange(true);
            }

            if (GameManager.villageManager.construct.CanReplaceBuilding(GameManager.uiManager.currentNormalBuidling.gameObject, selectedCell, gridMap))
            { constructCompo.yesButton.interactable = true; }
            else
            { constructCompo.yesButton.interactable = false; }
        }
        
    }

    public void ResetPrevTileColor()
    {
        foreach (var cell in previousHighlightedCells)
        {
            cell.RestoreTileColor();
        }
        previousHighlightedCells.Clear();
    }

    public bool IsBuildedBefore(int id)
    {
        int buildingID = id; //건물 ID
        var achieveID = 0;
        foreach (var achieve in DataTableManager.achievementTable.GetDatas())
        {
            if (achieve.TargetId == buildingID
                && achieve.AchieveType == ACHIEVEMENT_TYPE.BUILDING_BUILD)
                achieveID = achieve.AchieveId;
        }

        //관련 업적 없음
        if (achieveID == 0)
            return false;

        //업적 클리어됨
        if (GameManager.questManager.Achievements[achieveID] > 0)
            return true;

       return false;
    }
}
