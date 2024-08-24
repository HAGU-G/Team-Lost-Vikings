using System;
using System.Collections.Generic;
using TMPro;
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

    private int villagehallLv = 1;
    public int VillageHallLevel
    {
        get { return villagehallLv; }
        set { villagehallLv = value; }
    }

    private GameObject selectedObj;

    public ConstructMode constructMode = new();
    public Dictionary<Vector2Int, int> saveBuildingsData = new();

    private void Awake()
    {
        if (GameManager.villageManager != null)
        {
            Destroy(gameObject);
            return;
        }

        GameManager.villageManager = this;

        GameManager.Subscribe(EVENT_TYPE.INIT, OnGameInit);
        GameManager.Subscribe(EVENT_TYPE.START, OnGameStart);
    }


    private void OnGameInit()
    {
        Init();
        constructMode.Init();
    }

    private void OnGameStart()
    {
        //GameManager.Subscribe(EVENT_TYPE.QUIT, OnGameQuit);
        gridMap.ConcealGrid();
        gridMap.SetUsingTileList(GameManager.playerManager.level);

        if(GameManager.playerManager.firstPlay)
            VillageSet(gridMap);

    }

    //private void OnGameQuit()
    //{
    //    for(int i = 0; i < constructedBuildings.Count; ++i)
    //    {
    //        var building = constructedBuildings[i].GetComponent<Building>();
    //        var tileId = building.standardTile.tileInfo.id;
    //        var structureId = building.StructureId;

    //        saveBuildingsData.Add(tileId, structureId);
    //    }
    //}

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
        BuildingUpgrade upgradeComponent;
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
            buildingComponenet.StructureDesc = datas[i].StructureDesc;

            var sprite = b.GetComponent<SpriteRenderer>();
            upgradeComponent = b.AddComponent<BuildingUpgrade>();

            UpgradeData upgradeData = new();

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
            upgradeComponent.RequirePlayerLv = upgradeData.RequirePlayerLv;

            upgradeComponent.DropId = upgradeData.DropId;

            upgradeComponent.ItemIds = new();
            upgradeComponent.ItemNums = new();

            for (int j = 0; j < upgradeData.ItemIds.Count; ++j)
            {
                upgradeComponent.ItemIds.Add(upgradeData.ItemIds[j]);
                upgradeComponent.ItemNums.Add(upgradeData.ItemNums[j]);
            }

            upgradeComponent.UpgradeDesc = upgradeData.UpgradeDesc;
            upgradeComponent.StructureAssetFileName = upgradeData.StructureAssetFileName;

            var path = string.Concat(filePath, "/", upgradeComponent.StructureAssetFileName, ".prefab");
            var handle = Addressables.LoadAssetAsync<GameObject>(path);
            handle.WaitForCompletion(); //임시로 동기적 처리

            sprite.sprite = handle.Result.GetComponentInChildren<SpriteRenderer>().sprite;

            b.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            var collider = b.AddComponent<PolygonCollider2D>();

            switch (buildingComponenet.StructureType)
            {
                case STRUCTURE_TYPE.PARAMETER_RECOVERY:
                    var parameterComponent = b.AddComponent<ParameterRecoveryBuilding>();
                    parameterComponent.building = buildingComponenet;
                    parameterComponent.parameterType = (PARAMETER_TYPE)upgradeData.ParameterType;
                    parameterComponent.recoveryAmount = upgradeData.ParameterRecovery;
                    parameterComponent.rewardDropID = upgradeData.DropId;
                    parameterComponent.recoveryTime = upgradeData.RecoveryTime;
                    break;
                case STRUCTURE_TYPE.STAT_UPGRADE:
                    var statComponent = b.AddComponent<StatUpgradeBuilding>();
                    statComponent.building = buildingComponenet;
                    statComponent.upgradeStat = upgradeData.StatType;
                    statComponent.upgradeValue = upgradeData.StatReturn;
                    break;
                case STRUCTURE_TYPE.STANDARD:

                    break;
                case STRUCTURE_TYPE.PORTAL:
                    b.AddComponent<PortalBuilding>();
                    Destroy(collider);
                    break;
                case STRUCTURE_TYPE.REVIVE:
                    var revive = b.AddComponent<ReviveBuilding>();
                    revive.reviveTime = upgradeData.ProgressVarReturn;
                    break;
                case STRUCTURE_TYPE.PROGRESS:
                    if (upgradeData.ProgressVarType == (int)PROGRESS_TYPE.STORAGE)
                    {
                        var storage = b.AddComponent<StorageBuilding>();
                    }
                    else if (upgradeData.ProgressVarType == (int)PROGRESS_TYPE.HOTEL)
                    {
                        var hotel = b.AddComponent<HotelBuilding>();
                    }
                    else if (upgradeData.ProgressVarType == (int)PROGRESS_TYPE.RECRUIT)
                    {
                        var recruit = b.AddComponent<RecruitBuilding>();
                    }
                    break;
            }

            b.GetComponentInChildren<TextMeshPro>().text = buildingComponenet.StructureName;
            b.name = buildingComponenet.StructureId.ToString();
            installableBuilding.Add(b);
        }

    }

    public void LevelUp()
    {
        gridMap.SetUsingTileList(GameManager.playerManager.level);
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
        //selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.HP_RECOVERY);
        //var hp = construct.PlaceBuilding(selectedObj, GetTile(13, 19, gridMap), gridMap);

        //selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.STAMINA_RECOVERY);
        //var stamina = construct.PlaceBuilding(selectedObj, GetTile(13, 16, gridMap), gridMap);

        //selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.STRESS_RECOVERY);
        //var stress = construct.PlaceBuilding(selectedObj, GetTile(13, 13, gridMap), gridMap);

        //8,8 ~ 16,16

        selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.STANDARD);
        var standard = construct.PlaceBuilding(selectedObj, GetTile(12, 12, gridMap), gridMap);

        selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.PORTAL);
        var portal = construct.PlaceBuilding(selectedObj, GetTile(15, 15, gridMap), gridMap);
        var portalBuilding = portal.GetComponent<Building>();
        portalBuilding.RotateBuilding(portalBuilding);

        selectedObj = objectList.GetValueOrDefault((int)STRUCTURE_ID.REVIVE);
        construct.PlaceBuilding(selectedObj, GetTile(9, 15, gridMap), gridMap);

        SetDevelopText(false);
    }

    public void SetDevelopText(bool isOn)
    {
        foreach (var tile in gridMap.tiles)
        {
            var component = tile.Value.GetComponentInChildren<TextMeshPro>();
            if (component != null)
                component.enabled = isOn;
        }

        foreach (var building in constructedBuildings)
        {
            var component = building.GetComponentInChildren<TextMeshPro>();
            if (component != null)
                component.enabled = isOn;
        }

    }

    public bool FindBuilding(STRUCTURE_TYPE structureType, Predicate<GameObject> predicate)
    {
        if (constructedBuildings.Count <= 0
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

    public GameObject GetBuilding(STRUCTURE_ID id)
    {
        foreach (var tile in gridMap.tiles.Values)
        {
            if (!tile.tileInfo.ObjectLayer.IsEmpty)
            {
                if (tile.tileInfo.ObjectLayer.LayerObject.GetComponent<Building>().StructureId == (int)id)
                {
                    return tile.tileInfo.ObjectLayer.LayerObject;
                }
            }
        }

        Debug.Log("해당하는 id의 건물이 설치되지 않았습니다.");
        return null;
    }
}
