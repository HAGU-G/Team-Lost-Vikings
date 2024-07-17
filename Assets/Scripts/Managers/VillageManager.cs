﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.PackageManager;
using UnityEngine;

public class VillageManager : MonoBehaviour
{
    public Village village;
    public Construct construct;
    public List<GameObject> construectedBuildings = new();
    public GridMap gridMap;
    public Dictionary<int, GameObject> objectList = new();
    public List<GameObject> installableBuilding = new();
    public GameObject standardPrefab;
    public GameObject roadPrefab;

    private int playerLevel = 1;

    private GameObject selectedObj;

    private void Awake()
    {
        construct = gameObject.AddComponent<Construct>();
        
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
        //gridMap.SetUsingTileList(1);
        gridMap.SetUsingTileList(gridMap.usableTileList.Count -1);
        var standard = construct.ConstructStandardBuilding(standardPrefab, gridMap);
        construectedBuildings.Add(standard);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0f, 0f, 100f, 70f), "Remove Building"))
        {
            construct.isRemoveTime = true;
        }

        if (GUI.Button(new Rect(0f, 70f, 100f, 70f), "hp"))
        {
            selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.HP_RECOVERY);
            construct.isSelected = true;
        }

        if (GUI.Button(new Rect(0f, 140f, 100f, 70f), "stamina"))
        {
            selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.STAMINA_RECOVERY);
            construct.isSelected = true;
        }

        if (GUI.Button(new Rect(0f, 210f, 100f, 70f), "stress"))
        {
            selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.STRESS_RECOVERY);
            construct.isSelected = true;
        }

        if (GUI.Button(new Rect(0f, 350f, 100f, 70f), "Road"))
        {
            if(!construct.isRoadBuild)
                construct.isRoadBuild = true;
            else if(construct.isRoadBuild)
                construct.isRoadBuild = false;
        }

        if (GUI.Button(new Rect(0f, 420f, 100f, 70f), "Remove Road"))
        {
            if(!construct.isRoadRemove)
                construct.isRoadRemove = true;
            else if (construct.isRoadRemove)
                construct.isRoadRemove = false;
        }

        if (GUI.Button(new Rect(800f, 210f, 100f, 70f), "STR Upgrade"))
        {
            selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.STR_UPGRADE);
            construct.isSelected = true;
        }

        if (GUI.Button(new Rect(800f, 280f, 100f, 70f), "MAG Upgrade"))
        {
            selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.MAG_UPGRADE);
            construct.isSelected = true;
        }

        if (GUI.Button(new Rect(800f, 350f, 100f, 70f), "AGI Upgrade"))
        {
            selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.AGI_UPGRADE);
            construct.isSelected = true;
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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && construct.isSelected)
        {
            var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (GetTile(worldPos) != null)
            {
                var tile = GetTile(worldPos);
                var building = construct.PlaceBuilding(selectedObj, tile, gridMap);
                if (building == null)
                    return;
                construectedBuildings.Add(building);

                var statUpgrade = building.GetComponent<StatUpgradeBuilding>();
                if (statUpgrade != null)
                {
                    statUpgrade.SetUnits(village.units);
                    statUpgrade.RiseStat();
                }
            }
            else
            {
                construct.isSelected = false;
                return;
            }
        }

        if (Input.GetMouseButton(0) && construct.isRoadBuild)
        {
           
            var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (GetTile(worldPos) != null)
            {
                var tile = GetTile(worldPos);
                construct.PlaceRoad(roadPrefab, tile, gridMap); 
            }

        }

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

        if (Input.GetMouseButtonDown(0) && construct.isRoadRemove)
        {
            var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (GetTile(worldPos) != null)
            {
                var tile = GetTile(worldPos);
                construct.RemoveRoad(tile, gridMap);
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
        if(construectedBuildings.Count <= 0
            || construectedBuildings.FindIndex(predicate) < 0 ||
            construectedBuildings.FindIndex(predicate) >= construectedBuildings.Count)
        {
            Debug.Log("해당 건물이 없습니다.");
            return false;
        }

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
