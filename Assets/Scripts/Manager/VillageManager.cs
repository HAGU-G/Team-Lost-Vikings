using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
    public GameObject buildingPrefab;

    private int playerLevel = 1;

    private GameObject selectedObj;

    public int PlayerLevel { get { return playerLevel; } }

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
        Init();

        VillageSet(gridMap);
    }

    private void Init()
    {
        MakeBuildings();

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

        gridMap.SetUsingTileList(gridMap.usableTileList.Count - 1);
        //var standard = construct.ConstructStandardBuilding(standardPrefab, gridMap);
        //constructedBuildings.Add(standard);

        // village = gameObject.AddComponent<Village>();
    }

    private AsyncOperationHandle<GameObject> LoadBuildingObj(string path)
    {
        var obj = Addressables.LoadAssetAsync<GameObject>(path);
        obj.Completed += OnBuildingAssetLoaded;
        
        return obj;
    }

    public void OnBuildingAssetLoaded(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("AddressableAsset is not Loaded");
            return;
        }
    }

    private void MakeBuildings()
    {
        var datas = DataTableManager.buildingTable.GetDatas();
        string filePath = "Assets/Pick_Asset/2WEEK/Building";


        for (int i = 0; i < datas.Count; ++i)
        {
            var b = Instantiate(buildingPrefab);

            var buildingComponenet = b.AddComponent<Building>();
            buildingComponenet.StructureName = datas[i].StructureName;
            buildingComponenet.StructureId = datas[i].StructureId;
            buildingComponenet.Width = datas[i].Width;
            buildingComponenet.Length = datas[i].Length;
            buildingComponenet.StructureType = datas[i].StructureType;
            buildingComponenet.UnlockTownLevel = datas[i].UnlockTownLevel;
            buildingComponenet.CanMultiBuild = datas[i].CanMultiBuild;
            buildingComponenet.CanReverse = datas[i].CanReverse;
            buildingComponenet.CanReplace = datas[i].CanReplace;
            buildingComponenet.CanDestroy = datas[i].CanDestroy;
            buildingComponenet.UpgradeId = datas[i].UpgradeId;
            buildingComponenet.StructureAssetFileName = datas[i].StructureAssetFileName;
            buildingComponenet.StructureDesc = datas[i].StructureDesc;

            var sprite = b.GetComponent<SpriteRenderer>();
            var path = string.Concat(filePath, "/", buildingComponenet.StructureAssetFileName,".prefab");


            var handle = Addressables.LoadAssetAsync<GameObject>(path);
            handle.WaitForCompletion(); //임시로 동기적 처리

            sprite.sprite = handle.Result.GetComponentInChildren<SpriteRenderer>().sprite;

            UpgradeData upgradeData = new();

            if (buildingComponenet.UpgradeId != 0)
            {
                var upgradeComponent = b.AddComponent<BuildingUpgrade>();
                var dt = DataTableManager.upgradeTable.GetData(buildingComponenet.UpgradeId);
                upgradeComponent.UpgradeGrade = upgradeComponent.currentGrade;
                upgradeData = dt[upgradeComponent.UpgradeGrade - 1];

                upgradeComponent.StructureType = (int)buildingComponenet.StructureType;
                upgradeComponent.UpgradeName = upgradeData.UpgradeName;
                upgradeComponent.UpgradeId = buildingComponenet.UpgradeId;
                upgradeComponent.StatType = upgradeData.StatType;
                upgradeComponent.StatReturn = upgradeData.StatReturn;
                upgradeComponent.ParameterType = upgradeData.ParameterType;
                upgradeComponent.ParameterRecovery = upgradeData.ParameterRecovery;
                upgradeComponent.RecoveryTime = upgradeData.RecoveryTime;
                upgradeComponent.ProgressVarType = upgradeData.ProgressVarType;
                upgradeComponent.ProgressVarReturn = upgradeData.ProgressVarReturn;
                upgradeComponent.RecipeId = upgradeData.RecipeId;
                upgradeComponent.ItemStack = upgradeData.ItemStack;
                upgradeComponent.RequireTime = upgradeData.RequireTime;
                upgradeComponent.RequireGold = upgradeData.RequireGold;
                upgradeComponent.RequireRune = upgradeData.RequireRune;

                upgradeComponent.ItemIds = new();
                upgradeComponent.ItemNums = new();

                for (int j = 0; j < 5; ++j)
                {
                    upgradeComponent.ItemIds.Add(upgradeData.ItemIds[j]);
                    upgradeComponent.ItemNums.Add(upgradeData.ItemNums[j]);
                }

                upgradeComponent.UpgradeDesc = upgradeData.UpgradeDesc;


            }

            switch (buildingComponenet.StructureType)
            {
                case STRUCTURE_TYPE.PARAMETER_RECOVERY:
                    var parameterComponent = b.AddComponent<ParameterRecoveryBuilding>();
                    parameterComponent.building = buildingComponenet;
                    parameterComponent.parameterType = (PARAMETER_TYPE)upgradeData.ParameterType;
                    parameterComponent.recoveryAmount = upgradeData.ParameterRecovery;
                    parameterComponent.recoveryTime = upgradeData.RecoveryTime;
                    break;
                case STRUCTURE_TYPE.STAT_UPGRADE:
                    var statComponent = b.AddComponent<StatUpgradeBuilding>();
                    statComponent.building = buildingComponenet;
                    statComponent.upgradeStat = upgradeData.StatType;
                    statComponent.upgradeValue = upgradeData.StatReturn;
                    break;
                case STRUCTURE_TYPE.ITEM_SELL:
                    b.AddComponent<ItemSellBuilding>();
                    break;
                case STRUCTURE_TYPE.ITEM_PRODUCE:
                    b.AddComponent<ItemProduceBuilding>();
                    break;
                case STRUCTURE_TYPE.STANDARD:

                    break;
                case STRUCTURE_TYPE.PORTAL:
                    b.AddComponent<PortalBuilding>();
                    break;
            }

            b.GetComponentInChildren<TextMeshPro>().text = buildingComponenet.StructureName;
            b.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            b.AddComponent<PolygonCollider2D>();
            b.name = buildingComponenet.StructureId.ToString();
            installableBuilding.Add(b);
        }

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
        var stamina = construct.PlaceBuilding(selectedObj, GetTile(1, 4, gridMap), gridMap);
        constructedBuildings.Add(stamina);

        selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.STRESS_RECOVERY);
        var stress = construct.PlaceBuilding(selectedObj, GetTile(1, 1, gridMap), gridMap);
        constructedBuildings.Add(stress);

        selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.STANDARD);
        var standard = construct.PlaceBuilding(selectedObj, GetTile(4, 4, gridMap), gridMap);
        constructedBuildings.Add(standard);

        //selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.STR_UPGRADE);
        //var str = construct.PlaceBuilding(selectedObj, GetTile(1, 3, gridMap), gridMap);
        //constructedBuildings.Add(str);

        //selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.MAG_UPGRADE);
        //var mag = construct.PlaceBuilding(selectedObj, GetTile(1, 2, gridMap), gridMap);
        //constructedBuildings.Add(mag);

        //selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.AGI_UPGRADE);
        //var agi = construct.PlaceBuilding(selectedObj, GetTile(1, 1, gridMap), gridMap);
        //constructedBuildings.Add(agi);

        selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.PORTAL);
        var portal = construct.PlaceBuilding(selectedObj, GetTile(7, 5, gridMap), gridMap);
        constructedBuildings.Add(portal);

        var portalBuilding = portal.GetComponent<Building>();
        portalBuilding.RotateBuilding(portalBuilding);

        SetDevelopText(false);
    }

    public void SetDevelopText(bool isOn)
    {
        foreach (var building in constructedBuildings)
        {
            var component = building.GetComponentInChildren<TextMeshPro>();
            if (component != null)
                component.enabled = isOn;
        }

        foreach (var tile in gridMap.tiles)
        {
            var component = tile.Value.GetComponentInChildren<TextMeshPro>();
            if (component != null)
                component.enabled = isOn;
        }
    }

    public bool FindBuilding(STRUCTURE_TYPE structureType, Predicate<GameObject> predicate)
    {
        if(constructedBuildings.Count <= 0
            || constructedBuildings.FindIndex(predicate) < 0 
            || constructedBuildings.FindIndex(predicate) >= constructedBuildings.Count)
        {
            Debug.Log("해당 건물이 없습니다.");
            return false;
        }

        var building = constructedBuildings[constructedBuildings.FindIndex(predicate)];
        
        if (building.GetComponent<Building>().entranceTiles == null)
            return false;

        return true;
    }

    public GameObject FindBuildingEntrance(STRUCTURE_TYPE structureType, Predicate<GameObject> predicate)
    {
        var building = constructedBuildings[constructedBuildings.FindIndex(predicate)];
        return building;
    }
}
