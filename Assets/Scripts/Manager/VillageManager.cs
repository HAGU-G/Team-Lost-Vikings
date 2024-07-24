using System;
using System.Collections.Generic;
using UnityEngine;

public class VillageManager : MonoBehaviour
{
    public Village village;
    public Construct construct = new();
    public List<GameObject> constructedBuildings = new();
    public GridMap gridMap;
    public Dictionary<int, GameObject> objectList = new();
    public List<GameObject> installableBuilding = new();
    public GameObject standardPrefab;
    public GameObject roadPrefab;

    public List<GridMap> gridMaps;

    private int playerLevel = 1;

    private GameObject selectedObj;

    public int PlayerLevel {  get { return playerLevel; } }

    private void Awake()
    {
        if (GameManager.villageManager != null)
        {
            Destroy(gameObject);
            return;
        }

        GameManager.villageManager = this;

        GameManager.Subscribe(EVENT_TYPE.INIT, OnGameInit);
    }


    private void OnGameInit()
    {
        gridMaps = new List<GridMap>();
        //gridMap = new GridMap();
        //gridMap2 = new GridMap();

        Init();

        VillageSet(gridMap);
        //VillageSet(gridMap2);
    }

    private void Init()
    {
        foreach (var obj in installableBuilding)
        {
            var building = obj.GetComponent<Building>();
            objectList.Add(building.StructureId, obj);
        }
        //gridMap.SetUsingTileList(1);

        //foreach(var map in gridMaps)
        //{
        //    map.SetUsingTileList(map.usableTileList.Count - 1);
        //}

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
        Vector2 mousePos = GameManager.inputManager.WorldPos;
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

    public Cell GetTile(Vector3 position, GridMap map)
    {
            Vector2Int tileId = gridMap.PosToIndex(position);
            if (tileId.x >= 0 && tileId.y >= 0)
            {
                if (map.tiles.ContainsKey(tileId))
                {
                    return map.tiles[tileId];
                }
            }
        if (map.PosToIndex(position) == new Vector2Int(-1, -1))
            return null;

        return null;
    }

    public Cell GetTile(int x, int y, GridMap includedGridMap)
    {
        Cell tile = includedGridMap.GetTile(x, y);
        if (tile != null)
        {
            return tile;
        }
        Debug.Log("타일을 찾을 수 없습니다.");
        return null;
    }

    private void Update()
    {
        if (GameManager.inputManager.Press)
        {
            Vector2 mousePos = GameManager.inputManager.WorldPos;
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 100f);

            Debug.Log(GameManager.uiManager.isWindowOn);
            if (GameManager.uiManager.isWindowOn)
                return;
            Debug.Log("확인용");
            if (hit.collider != null)
            {
                Debug.Log(hit);
                var building = hit.transform.gameObject.GetComponent<Building>();
                var parameter = hit.transform.gameObject.GetComponent<ParameterRecoveryBuilding>();
                if(parameter != null)
                {
                    parameter.TouchParameterBuilding();
                }
                else if(building != null)
                {
                    building.TouchBuilding();
                }
            }
        }

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

    private void VillageSet(GridMap gridMap)
    {
        for (int i = 0; i < 9; ++i)
        {
            construct.PlaceRoad(roadPrefab, GetTile(0, i, gridMap), gridMap);
            construct.PlaceRoad(roadPrefab, GetTile(4, i, gridMap), gridMap);
            construct.PlaceRoad(roadPrefab, GetTile(8, i, gridMap), gridMap);
        }

        for(int i = 0; i < 9; ++i)
        {
            construct.PlaceRoad(roadPrefab, GetTile(i, 0, gridMap), gridMap);
            construct.PlaceRoad(roadPrefab, GetTile(i, 4, gridMap), gridMap);
            construct.PlaceRoad(roadPrefab, GetTile(i, 8, gridMap), gridMap);
        }

        construct.PlaceRoad(roadPrefab, GetTile(3, 3, gridMap), gridMap);
        construct.PlaceRoad(roadPrefab, GetTile(5, 3, gridMap), gridMap);
        construct.PlaceRoad(roadPrefab, GetTile(3, 5, gridMap), gridMap);
        construct.PlaceRoad(roadPrefab, GetTile(5, 5, gridMap), gridMap);


        /////////
        selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.HP_RECOVERY);
        var hp = construct.PlaceBuilding(selectedObj, GetTile(1, 7, gridMap), gridMap);
        constructedBuildings.Add(hp);

        selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.STAMINA_RECOVERY);
        var stamina = construct.PlaceBuilding(selectedObj, GetTile(1, 6, gridMap), gridMap);
        constructedBuildings.Add(stamina);

        selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.STRESS_RECOVERY);
        var stress = construct.PlaceBuilding(selectedObj, GetTile(1, 5, gridMap), gridMap);
        constructedBuildings.Add(stress);

        selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.STANDARD);
        var standard = construct.PlaceBuilding(selectedObj, GetTile(4, 4, gridMap), gridMap);
        constructedBuildings.Add(standard);

        selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.STR_UPGRADE);
        var str = construct.PlaceBuilding(selectedObj, GetTile(1, 3, gridMap), gridMap);
        //str.GetComponent<StatUpgradeBuilding>().RiseStat(); // StatUpgradeBuilding이 처리하도록 변경
        constructedBuildings.Add(str);

        selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.MAG_UPGRADE);
        var mag = construct.PlaceBuilding(selectedObj, GetTile(1, 2, gridMap), gridMap);
        //mag.GetComponent<StatUpgradeBuilding>().RiseStat();
        constructedBuildings.Add(mag);

        selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.AGI_UPGRADE);
        var agi = construct.PlaceBuilding(selectedObj, GetTile(1, 1, gridMap), gridMap);
        //agi.GetComponent<StatUpgradeBuilding>().RiseStat();
        constructedBuildings.Add(agi);
        
        selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.PORTAL);
        var portal = construct.PlaceBuilding(selectedObj, GetTile(7, 5, gridMap), gridMap);
        constructedBuildings.Add(portal);

        var portalBuilding = portal.GetComponent<Building>();
        portalBuilding.RotateBuilding(portalBuilding);

        gridMaps.Add(gridMap);
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
        return building;
    }
}
