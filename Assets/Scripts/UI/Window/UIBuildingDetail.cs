using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
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

    private List<UpgradeData> grade = new();
    public List<int> requireItemIds;
    public List<int> requireItemNums;

    private bool isOpen = false;


    //private bool isDragging = false;

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

        

        GameManager.Subscribe(EVENT_TYPE.CONFIGURE, OnGameConfigure);
    }

    private void OnEnable()
    {
        isOpen = true;

        um.uiDevelop.ConstructButtonsOff();
    }

    private void OnGameConfigure()
    {
        im.OnItemChangedCallback += OnItemChanged;
    }

    private void OnItemChanged()
    {
        if (isOpen)
            SetResourceText();
    }

    private void SetResourceText()
    {
        grade = DataTableManager.upgradeTable.GetData(um.currentBuildingData.UpgradeId);
        requireItemIds = grade[0].ItemIds;
        requireItemNums = grade[0].ItemNums;

        for (int i = 0; i < resources.Count; ++i)
        {
            resources[i].GetComponentInChildren<TextMeshProUGUI>().text = $"{im.GetItem(requireItemIds[i])} / {requireItemNums[i]}";
        }

        CheckRequireItems();
    }

    public void SetBuildingDetail(bool canBuild)
    {
        var buildingData = um.currentBuildingData;

        var upgrade = DataTableManager.upgradeTable.GetData(buildingData.UpgradeId)[0];

        buildingName.text = buildingData.StructureName.ToString();

        string assetName = upgrade.StructureAssetFileName;
        var path = $"Assets/Pick_Asset/2WEEK/Building/{assetName}.prefab"; //TO-DO : 파일 경로 수정하기

        var handle = Addressables.LoadAssetAsync<GameObject>(path);
        handle.WaitForCompletion();

        buildingImage.sprite = handle.Result.GetComponentInChildren<SpriteRenderer>().sprite;

        unlockTownLevel.text = $"요구 마을회관 레벨 : {buildingData.UnlockTownLevel.ToString()}";
        buidlingSize.text = $"{buildingData.Width} X {buildingData.Length}";
        buildingDesc.text = buildingData.StructureDesc.ToString();

        foreach (var r in resources)
        {
            Destroy(r);
        }
        resources.Clear();
        //TO-DO : 재설치 텍스트 지우는 내용 추가하기

        if(!vm.construct.IsBuildedBefore(buildingData.StructureId))
        {
            for (int i = 0; i < upgrade.ItemIds.Count; ++i)
            {
                var resource = GameObject.Instantiate(upgradeResource, requireTransform);

                var asset = upgrade.ItemIds[i];
                //resource.GetComponentInChildren<Image>().sprite = ;

                resource.GetComponentInChildren<TextMeshProUGUI>().text
                    = $"{im.GetItem(upgrade.ItemIds[i])} / {upgrade.ItemNums[i]}";

                resources.Add(resource);
            }
        }
        else
        {
            //TO-DO : UI변경되면 텍스트 넣는 부분 추가하기
        }
        
        CheckRequireItems();
    }

    private void CheckRequireItems()
    {
        var buildingData = um.currentBuildingData;
        var upgrade = DataTableManager.upgradeTable.GetData(buildingData.UpgradeId)[0];
        var requireItemIds = upgrade.ItemIds;
        var requireItemNums = upgrade.ItemNums;

        bool check = true;

        Color trueColor = new Color(200f / 255f, 231f/55f, 167f / 255f);
        Color falseColor = new Color(255f / 255f, 128f/55f, 128f / 255f);

        for (int i = 0; i < resources.Count; ++i)
        {
            if (requireItemNums[i] <= im.GetItem(requireItemIds[i]))
            {
                resources[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
            }
            else
            {
                resources[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
                check = false;
            }
        }

        foreach (var tile in vm.gridMap.tiles.Values)
        {
            if (!tile.tileInfo.ObjectLayer.IsEmpty
                && tile.tileInfo.ObjectLayer.LayerObject.GetComponentInChildren<Building>().StructureId == buildingData.StructureId
                && !buildingData.CanMultiBuild)
            {
                check = false;
            }
        }

        if(buildingData.UnlockTownLevel > vm.VillageHallLevel)
        {
            check = false;
        }

        if (check)
            construct.targetGraphic.color = trueColor;
        else
            construct.targetGraphic.color = falseColor;

        construct.interactable = check;
        
        //if(canBuild != true)
        //    construct.interactable = canBuild;
    }

    public void OnButtonConstruct()
    {
        GameManager.PlayButtonSFX();
        exceptWindows[0] = um.windows[WINDOW_NAME.CONSTRUCT_MODE];
        um.CloseWindows(exceptWindows);
        //um.uiDevelop.ConstructButtonsOff();
        UIConstructMode constructMode = um.windows[WINDOW_NAME.CONSTRUCT_MODE] as UIConstructMode;
        constructMode.isConstructing = true;
        vm.construct.MakeBuildingGrid();
    }

    public GameObject ConstructBuilding(Cell cell)
    {
        if (vm.objectList.TryGetValue(um.currentBuildingData.StructureId, out var building))
        {
            var obj = vm.constructMode.construct.PlaceBuilding(building, cell, vm.gridMap);
            if (obj == null)
                return null;
            obj.GetComponentInChildren<TextMeshPro>().enabled = false;

            //업적 카운팅
            var buildingID = building.GetComponent<Building>().StructureId;
            GameManager.questManager.SetAchievementCountByTargetID(buildingID, ACHIEVEMENT_TYPE.BUILDING_BUILD, 1);

            return obj;
        }
        return null;
    }

    public void OnButtonExit()
    {
        GameManager.PlayButtonSFX();
        exceptWindows[0] = um.windows[WINDOW_NAME.CONSTRUCT_MODE];
        um.CloseWindows(exceptWindows);
        isOpen = false;
    }
}
