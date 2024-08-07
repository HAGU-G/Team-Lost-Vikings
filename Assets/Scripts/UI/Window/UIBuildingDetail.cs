using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
public class UIBuildingDetail : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.BUILDING_DETAIL;

    public List<BuildingData> buildingDatas = new();
    public List<UpgradeData> upgradeDatas = new();

    public ItemManager im;
    public UIManager um;
    public VillageManager vm;

    UIWindow[] exceptWindows;

    public TextMeshProUGUI buildingName;
    public Image buildingImage;
    public TextMeshProUGUI unlockTownLevel;
    public TextMeshProUGUI buidlingSize;
    public TextMeshProUGUI buildingDesc;

    public GameObject upgradeResource;
    public Button construct;
    public Button exit;
    public List<GameObject> resources = new();
    public Transform requireTransform;

    private Vector3 position;
    public bool isConstructing = false;

    //private bool isDragging = false;
    List<Cell> buildingCells = new List<Cell>();

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnGameStart()
    {
        base.OnGameStart();
        isShowOnly = false;

        im = GameManager.itemManager;
        um = GameManager.uiManager;
        vm = GameManager.villageManager;

        exceptWindows = new UIWindow[20];

        buildingDatas = DataTableManager.buildingTable.GetDatas();
        upgradeDatas = DataTableManager.upgradeTable.GetDatas();
    }

    public void SetBuildingDetail()
    {
        var buildingData = um.currentBuildingData;

        var upgrade = DataTableManager.upgradeTable.GetData(buildingData.UpgradeId)[1];

        buildingName.text = buildingData.StructureName.ToString();

        string assetName = buildingData.StructureAssetFileName;
        var path = $"Assets/Pick_Asset/2WEEK/Building/{assetName}.prefab"; //TO-DO : 파일 경로 수정하기

        var handle = Addressables.LoadAssetAsync<GameObject>(path);
        handle.WaitForCompletion();

        buildingImage.sprite = handle.Result.GetComponentInChildren<SpriteRenderer>().sprite;
        
        unlockTownLevel.text = buildingData.UnlockTownLevel.ToString();
        //buidlingSize.text = $"{buildingData.Width} X {buildingData.Length}";
        buildingDesc.text = buildingData.StructureDesc.ToString();

        foreach (var r in resources)
        {
            Destroy(r);
        }
        resources.Clear();

        for (int i = 0; i < upgrade.ItemNums.Count; ++i)
        {
            var resource = GameObject.Instantiate(upgradeResource, requireTransform);
            //resource.GetComponentInChildren<Image>().sprite = ; //이미지 박아 놓는건지 가변적인지 물어보기

            //TO-DO : 아이템 추가되면 주석 해제하기
            //resource.GetComponentInChildren<TextMeshProUGUI>().text
            //    = $"{im.ownItemList[upgrade.ItemIds[i]]} / {upgrade.ItemNums[upgrade.ItemIds[i]].ToString()}";

            resources.Add(resource);
        }
    }

    private void CheckRequireItems()
    {
        for (int i = 0; i < resources.Count; ++i)
        {

        }
    }

    public void OnButtonConstruct()
    {
        exceptWindows[0] = um.windows[WINDOW_NAME.CONSTRUCT_MODE];
        um.CloseWindows(exceptWindows);
        isConstructing = true;
    }

    public GameObject ConstructBuilding(Cell cell)
    {
        if (vm.objectList.TryGetValue(um.currentNormalBuidling.StructureId, out var building))
        {
            var obj = vm.constructMode.construct.PlaceBuilding(building, cell, vm.gridMap);
            if (obj == null)
                return null;
            obj.GetComponentInChildren<TextMeshPro>().enabled = false;
            return obj;
        }
        return null;
    }

    private void MakeBuildingGrid()
    {
        var width = um.currentBuildingData.Width;
        var length = um.currentBuildingData.Length;
        var gridMap = GameManager.villageManager.gridMap;

        Vector3 centerPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));
        Vector2Int centerIndex = gridMap.PosToIndex(centerPos);

        CreateBuildingGrid(centerIndex, width, length);
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
                    buildingCells.Add(cell);
                    cell.GetComponent<SpriteRenderer>().color = Color.green; // 임시 색상
                }
            }
        }
    }

    public void OnButtonExit()
    {
        exceptWindows[0] = um.windows[WINDOW_NAME.CONSTRUCT_MODE];
        um.CloseWindows(exceptWindows);
    }
}
