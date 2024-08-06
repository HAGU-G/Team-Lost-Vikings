using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
public class UIBuildingDetail : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.BUILDING_DETAIL;

    public List<BuildingData> buildingDatas = new();
    public List<UpgradeData> upgradeDatas = new();

    public ItemManager im;
    public UIManager um;

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

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnGameStart()
    {
        base.OnGameStart();
        im = GameManager.itemManager;
        um = GameManager.uiManager;
        buildingDatas = DataTableManager.buildingTable.GetDatas();
        upgradeDatas = DataTableManager.upgradeTable.GetDatas();
    }

    private void OnEnable()
    {
        SetBuildingDetail();
    }

    private void SetBuildingDetail()
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
            resources.Remove(r);
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

    }

    public void OnButtonExit()
    {
        Close();
    }
}
