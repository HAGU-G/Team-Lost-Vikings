using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;

public class VillageManager : MonoBehaviour
{
    public List<GameObject> construectedBuildings = new List<GameObject>();
    public GridMap gridMap;
    private bool isSelected = false;
    private bool isRemoveTime = false;
    public Dictionary<int, GameObject> objectList = new Dictionary<int, GameObject>();
    public List<GameObject> installableBuilding = new();
    public GameObject hospital;

    private GameObject selectedObj;

    private void Awake()
    {
        GameStarter.Instance.SetActiveOnComplete(gameObject);
    }

    private void Start()
    {
        foreach(var obj in installableBuilding)
        {
            var building = obj.GetComponent<Building>();
            objectList.Add(building.StructureId, obj);
        }
        
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0f, 150f, 70f, 70f), "hp"))
        {
            selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.HP_RECOVERY);
            isSelected = true;
        }

        if (GUI.Button(new Rect(0f, 220, 70f, 70f), "stamina"))
        {
            selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.STAMINA_RECOVERY);
            isSelected = true;
        }

        if (GUI.Button(new Rect(0f, 290, 70f, 70f), "stress"))
        {
            selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.STRESS_RECOVERY);
            isSelected = true;
        }

        if (GUI.Button(new Rect(0f, 80f, 70f, 70f), "Remove"))
        {
            isRemoveTime = true;
        }
    }

    public void PlaceBuilding(GameObject obj, Tile tile)
    {
        var objInfo = obj.GetComponent<Building>();
        var width = objInfo.Width;
        var height = objInfo.Length;

        objInfo.placedTiles.Clear();
        var tileId = tile.tileInfo.id;
        var indexX = tileId.x + width - 1;
        var indexY = tileId.y + height - 1;

        if (indexX < 0 || indexY < 0 || indexX > gridMap.gridInfo.row || indexY > gridMap.gridInfo.col)
        {
            Debug.Log("건물을 설치할 수 없습니다.");
            isSelected = false;
            return;
        }

        objInfo.entranceTile = gridMap.tiles[new Vector2Int(tile.tileInfo.id.x -1, tile.tileInfo.id.y)];
        var instancedObj = Instantiate(obj, gridMap.IndexToPos(tileId), Quaternion.identity, tile.transform);
        var pos = instancedObj.transform.position; 
        pos.y = instancedObj.transform.position.y - gridMap.gridInfo.cellSize / 4f;
        instancedObj.transform.position = pos;
        construectedBuildings.Add(instancedObj);
        var buildingComponent = instancedObj.GetComponent<Building>();
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
    }

    public void RemoveBuilding(Tile tile)
    {
        var obj = tile.tileInfo.ObjectLayer.LayerObject;
        var objInfo = obj.GetComponent<Building>();
        var width = objInfo.Width;
        var height = objInfo.Length;

        if (objInfo.placedTiles == null || !objInfo.placedTiles.Any()) 
            return;

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
        construectedBuildings.Remove(obj);
        Destroy(obj);
        tile.ResetTileInfo();
        isRemoveTime = false;
    }

    private void InteractWithBuilding()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 100f);

        if (hit.collider != null)
        {
            Building building = hit.transform.gameObject.GetComponent<Building>();
            if (building != null)
            {
                building.Interact();
            }
        }
    }

    public void PlaceRoad(Tile tile)
    {
        //tile.tileInfo.ObjectLayer = 
        //tile.UpdateTileInfo(TileType.ROAD, tilePrefab);
    }

    public Tile GetTile(Vector3 position)
    {
        var tileId = gridMap.PosToIndex(position);
        if (tileId.x < 0 || tileId.y < 0)
            return null;
        return gridMap.tiles[tileId];
    }

    public Tile GetTile(Vector2Int vector)
    {
        return gridMap.tiles[vector];
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && isSelected)
        {
            var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var tile = GetTile(worldPos);
            PlaceBuilding(selectedObj, tile);
        }

        //if (Input.GetMouseButtonDown(0) && !isSelected)
        //{
        //    var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    if(GetTile(worldPos) != null)
        //        InteractWithBuilding();
        //}

        //if (Input.GetMouseButtonDown(0))
        //{
        //    var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    var tile = GetTile(worldPos);
        //    Debug.Log(tile.tileInfo.TileType.ToString());
        //}

        //if (Input.GetMouseButtonDown(0) && isRemoveTime)
        //{
        //    var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    var tile = GetTile(worldPos);
        //    RemoveBuilding(tile);
        //}
    }

    public bool FindBuilding(STRUCTURE_TYPE structureType, Predicate<GameObject> predicate)
    {
        if (construectedBuildings.Count <= 0)
            return false;
        Debug.Log(construectedBuildings.FindIndex(predicate));
        var building = construectedBuildings[construectedBuildings.FindIndex(predicate)];
        var tile = building.GetComponent<Building>().entranceTile;
        if (tile == null)
            return false;

        return true;
    }

    public Tile FindBuildingEntrance(STRUCTURE_TYPE structureType, Predicate<GameObject> predicate)
    {
       
        var building = construectedBuildings[construectedBuildings.FindIndex(predicate)];
        var tile = building.GetComponent<Building>().entranceTile;
        Debug.Log($"입구 타일의 인덱스 : {tile.tileInfo.id.x}, {tile.tileInfo.id.y}");
        return building.GetComponent<Building>().entranceTile;
    }
}
