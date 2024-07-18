using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.PackageManager;
using UnityEngine;

public class VillageManager : MonoBehaviour
{
    public bool isShowing = true;

    public Village village;
    public Construct construct = new();
    public List<GameObject> constructedBuildings = new();
    public GridMap gridMap;
    public Dictionary<int, GameObject> objectList = new();
    public List<GameObject> installableBuilding = new();
    public GameObject standardPrefab;
    public GameObject roadPrefab;

    private int playerLevel = 1;

    private GameObject selectedObj;



    //public UnitOnVillage GetUnit(UnitStats unitStats)
    //{
    //    var unit = village.units.First();
    //    unit.ResetUnit(unitStats);

    //    unit.stats.ResetStats();
    //    unit.gameObject.transform.SetParent(gridMap.transform);
    //    unit.gameObject.SetActive(true);

    //    if (!village.units.Contains(unit))
    //        village.units.Add(unit);

    //    return unit;
    //}

    private void Awake()
    {
        if (GameManager.villageManager != null)
        {
            Destroy(gameObject);
            return;
        }

        GameManager.villageManager = this;
    }


    private void Start()
    {
        Init();

        VillageSet();
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

        //var standard = construct.ConstructStandardBuilding(standardPrefab, gridMap);
        //constructedBuildings.Add(standard);

       // village = gameObject.AddComponent<Village>();
    }

    private void OnGUI()
    {
        //if (GUI.Button(new Rect(0f, 0f, 100f, 70f), "Remove Building"))
        //{
        //    construct.isRemoveTime = true;
        //}

        //if (GUI.Button(new Rect(0f, 70f, 100f, 70f), "hp"))
        //{
        //    selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.HP_RECOVERY);
        //    construct.isSelected = true;
        //}

        //if (GUI.Button(new Rect(0f, 140f, 100f, 70f), "stamina"))
        //{
        //    selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.STAMINA_RECOVERY);
        //    construct.isSelected = true;
        //}

        //if (GUI.Button(new Rect(0f, 210f, 100f, 70f), "stress"))
        //{
        //    selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.STRESS_RECOVERY);
        //    construct.isSelected = true;
        //}

        //if (GUI.Button(new Rect(0f, 350f, 100f, 70f), "Road"))
        //{
        //    if(!construct.isRoadBuild)
        //        construct.isRoadBuild = true;
        //    else if(construct.isRoadBuild)
        //        construct.isRoadBuild = false;
        //}

        //if (GUI.Button(new Rect(0f, 420f, 100f, 70f), "Remove Road"))
        //{
        //    if(!construct.isRoadRemove)
        //        construct.isRoadRemove = true;
        //    else if (construct.isRoadRemove)
        //        construct.isRoadRemove = false;
        //}

        //if (GUI.Button(new Rect(800f, 210f, 100f, 70f), "STR Upgrade"))
        //{
        //    selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.STR_UPGRADE);
        //    construct.isSelected = true;
        //}

        //if (GUI.Button(new Rect(800f, 280f, 100f, 70f), "MAG Upgrade"))
        //{
        //    selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.MAG_UPGRADE);
        //    construct.isSelected = true;
        //}

        //if (GUI.Button(new Rect(800f, 350f, 100f, 70f), "AGI Upgrade"))
        //{
        //    selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.AGI_UPGRADE);
        //    construct.isSelected = true;
        //}

    }

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

    public Cell GetTile(Vector3 position)
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
        //if (Input.GetMouseButtonDown(0) && construct.isSelected)
        //{
        //    var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    if (GetTile(worldPos) != null)
        //    {
        //        var tile = GetTile(worldPos);
        //        var building = construct.PlaceBuilding(selectedObj, tile, gridMap);
        //        if (building == null)
        //            return;
        //        constructedBuildings.Add(building);

        //        var statUpgrade = building.GetComponent<StatUpgradeBuilding>();
        //        if (statUpgrade != null)
        //        {
        //            statUpgrade.SetUnits(village.units);
        //            statUpgrade.RiseStat();
        //        }
        //    }
        //    else
        //    {
        //        construct.isSelected = false;
        //        return;
        //    }
        //}

        //if (Input.GetMouseButton(0) && construct.isRoadBuild)
        //{
           
        //    var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    if (GetTile(worldPos) != null)
        //    {
        //        var tile = GetTile(worldPos);
        //        construct.PlaceRoad(roadPrefab, tile, gridMap); 
        //    }

        //}

        //if (Input.GetMouseButtonDown(0) && construct.isRemoveTime)
        //{
        //    var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    if (GetTile(worldPos) != null)
        //    {
        //        var tile = GetTile(worldPos);
        //        var building = construct.RemoveBuilding(tile, gridMap);
        //        constructedBuildings.Remove(building);
        //    }
        //    else
        //    {
        //        Debug.Log("잘못된 인덱스 선택");
        //        construct.isRemoveTime = false;
        //    }
        //}

        //if (Input.GetMouseButtonDown(0) && construct.isRoadRemove)
        //{
        //    var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    if (GetTile(worldPos) != null)
        //    {
        //        var tile = GetTile(worldPos);
        //        construct.RemoveRoad(tile, gridMap);
        //    }
        //    else
        //    {
        //        Debug.Log("잘못된 인덱스 선택");
        //        construct.isRemoveTime = false;
        //    }
        //}
    }

    private void VillageSet()
    {
        for (int i = 27; i < 36; ++i)
        {
            construct.PlaceRoad(roadPrefab, gridMap.GetTile(36, i), gridMap);
            construct.PlaceRoad(roadPrefab, gridMap.GetTile(32, i), gridMap);
            construct.PlaceRoad(roadPrefab, gridMap.GetTile(28, i), gridMap);
        }

        for(int i = 29; i < 36; ++i)
        {
            construct.PlaceRoad(roadPrefab, gridMap.GetTile(i, 35), gridMap);
            construct.PlaceRoad(roadPrefab, gridMap.GetTile(i, 31), gridMap);
            construct.PlaceRoad(roadPrefab, gridMap.GetTile(i, 27), gridMap);
        }

        construct.PlaceRoad(roadPrefab, gridMap.GetTile(31, 32), gridMap);
        construct.PlaceRoad(roadPrefab, gridMap.GetTile(31, 30), gridMap);
        construct.PlaceRoad(roadPrefab, gridMap.GetTile(33, 30), gridMap);
        construct.PlaceRoad(roadPrefab, gridMap.GetTile(33, 32), gridMap);


        /////////
        selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.HP_RECOVERY);
        var hp = construct.PlaceBuilding(selectedObj, gridMap.GetTile(29, 34), gridMap);
        constructedBuildings.Add(hp);

        selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.STAMINA_RECOVERY);
        var stamina = construct.PlaceBuilding(selectedObj, gridMap.GetTile(29, 33), gridMap);
        constructedBuildings.Add(stamina);

        selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.STRESS_RECOVERY);
        var stress = construct.PlaceBuilding(selectedObj, gridMap.GetTile(29, 32), gridMap);
        constructedBuildings.Add(stress);

        selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.STR_UPGRADE);
        var str = construct.PlaceBuilding(selectedObj, gridMap.GetTile(29, 30), gridMap);
        constructedBuildings.Add(str);

        selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.STANDARD);
        var standard = construct.PlaceBuilding(selectedObj, gridMap.GetTile(32, 31), gridMap);
        constructedBuildings.Add(standard);

        selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.MAG_UPGRADE);
        var mag = construct.PlaceBuilding(selectedObj, gridMap.GetTile(29, 29), gridMap);
        constructedBuildings.Add(mag);

        selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.AGI_UPGRADE);
        var agi = construct.PlaceBuilding(selectedObj, gridMap.GetTile(29, 28), gridMap);
        constructedBuildings.Add(agi);

        selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.PORTAL);
        var portal = construct.PlaceBuilding(selectedObj, gridMap.GetTile(35, 32), gridMap);
        constructedBuildings.Add(portal);

        var portalBuilding = portal.GetComponent<Building>();
        portalBuilding.RotateBuilding(portalBuilding);

    }

    public bool FindBuilding(STRUCTURE_TYPE structureType, Predicate<GameObject> predicate)
    {
        if(constructedBuildings.Count <= 0
            || constructedBuildings.FindIndex(predicate) < 0 ||
            constructedBuildings.FindIndex(predicate) >= constructedBuildings.Count)
        {
            Debug.Log("해당 건물이 없습니다.");
            return false;
        }

        var building = constructedBuildings[constructedBuildings.FindIndex(predicate)];
        var tile = building.GetComponent<Building>().entranceTile;
        if (tile == null)
            return false;

        return true;
    }

    public GameObject FindBuildingEntrance(STRUCTURE_TYPE structureType, Predicate<GameObject> predicate)
    {

        var building = constructedBuildings[constructedBuildings.FindIndex(predicate)];
        var tile = building.GetComponent<Building>().entranceTile;
        Debug.Log($"입구 타일의 인덱스 : {tile.tileInfo.id.x}, {tile.tileInfo.id.y}");
        return building;
    }
}
