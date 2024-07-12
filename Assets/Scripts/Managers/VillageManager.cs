using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.PackageManager;
using UnityEngine;

public class VillageManager : MonoBehaviour
{
    private ConstructBuilding construct = new();
    public List<GameObject> construectedBuildings = new();
    public GridMap gridMap;
    public Dictionary<int, GameObject> objectList = new();
    public List<GameObject> installableBuilding = new();
    public GameObject standardBuilding;

    private int playerLevel = 1;

    private GameObject selectedObj;

    private void Awake()
    {
        GameStarter.Instance.SetActiveOnComplete(gameObject);
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        foreach (var obj in installableBuilding)
        {
            var building = obj.GetComponent<Building>();
            objectList.Add(building.StructureId, obj);
        }
        gridMap.SetUsingTileList(1);
        var standard = construct.ConstructStandardBuilding(standardBuilding, gridMap);
        construectedBuildings.Add(standard);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0f, 150f, 70f, 70f), "hp"))
        {
            selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.HP_RECOVERY);
            construct.isSelected = true;
        }

        if (GUI.Button(new Rect(0f, 220, 70f, 70f), "stamina"))
        {
            selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.STAMINA_RECOVERY);
            construct.isSelected = true;
        }

        if (GUI.Button(new Rect(0f, 290, 70f, 70f), "stress"))
        {
            selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.STRESS_RECOVERY);
            construct.isSelected = true;
        }

        if (GUI.Button(new Rect(0f, 80f, 70f, 70f), "Remove"))
        {
            construct.isRemoveTime = true;
        }
    }

    //private void OnEntranceTileChanged(Tile tile)
    //{
    //    if (gridMap.tiles.TryGetValue(tile.tileInfo.id, out Tile gridTile))
    //    {
    //        var building = gridTile.tileInfo.ObjectLayer.LayerObject.GetComponent<Building>();
    //        building.entranceTile = tile;
    //    }
    //}


    public void LevelUp()
    {
        ++playerLevel;
        gridMap.SetUsingTileList(playerLevel);

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
        if (gridMap.PosToIndex(position) == new Vector2Int(-1, -1))
            return null;

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
        if (Input.GetMouseButtonDown(0) && construct.isSelected)
        {
            var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (GetTile(worldPos) != null)
            {
                var tile = GetTile(worldPos);
                if (!gridMap.usingTileList.Contains(tile))
                {
                    Debug.Log("확장되지 않은 영역에 설치를 시도했습니다.");
                    construct.isSelected = false;
                    return;
                }
                var building = construct.PlaceBuilding(selectedObj, tile, gridMap);
                construectedBuildings.Add(building);
            }
            else
            {
                construct.isSelected = false;
                return;
            }
        }

        if(Input.GetKeyDown(KeyCode.U))
        {
            LevelUp();
        }

        //if (Input.GetMouseButtonDown(0) && !isSelected)
        //{
        //    var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    if (GetTile(worldPos) != null)
        //        InteractWithBuilding();
        //}

        //if (Input.GetMouseButtonDown(0))
        //{
        //    var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    if(GetTile(worldPos) != null)
        //    {
        //        var tile = GetTile(worldPos);
        //        Debug.Log(tile.tileInfo.TileType.ToString());
        //    }
        //}

        if (Input.GetMouseButtonDown(0) && construct.isRemoveTime)
        {
            var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (GetTile(worldPos) != null)
            {
                var tile = GetTile(worldPos);
                var building = construct.RemoveBuilding(tile, gridMap);
                construectedBuildings.Remove(building);

            }
            else
            {
                Debug.Log("잘못된 인덱스 선택");
                construct.isRemoveTime = false;
            }

        }
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

    public GameObject FindBuildingEntrance(STRUCTURE_TYPE structureType, Predicate<GameObject> predicate)
    {

        var building = construectedBuildings[construectedBuildings.FindIndex(predicate)];
        var tile = building.GetComponent<Building>().entranceTile;
        Debug.Log($"입구 타일의 인덱스 : {tile.tileInfo.id.x}, {tile.tileInfo.id.y}");
        return building;
    }
}
