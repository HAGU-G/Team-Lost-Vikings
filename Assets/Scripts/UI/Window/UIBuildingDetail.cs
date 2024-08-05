using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIBuildingDetail : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.BUILDING_DETAIL;

    public List<BuildingData> buildingDatas = DataTableManager.buildingTable.GetDatas();
    public List<UpgradeData> upgradeDatas = DataTableManager.upgradeTable.GetDatas();

    public ItemManager im;

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
    }

    private void OnEnable()
    {
        SetBuildingDetail();
    }

    private void SetBuildingDetail()
    {
        foreach (var buildingData in buildingDatas)
        {
            var upgrade = upgradeDatas[buildingData.UpgradeId];

            buildingName.text = buildingData.StructureName.ToString();
            //detail.buildingImage = upgrade. //데이터테이블 매니저 수정 후 적기
            unlockTownLevel.text = buildingData.UnlockTownLevel.ToString();
            buidlingSize.text = $"{buildingData.Width} X {buildingData.Length}";
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
                resource.GetComponentInChildren<TextMeshProUGUI>().text
                    = $"{im.ownItemList[upgrade.ItemIds[i]]} / {upgrade.ItemNums[upgrade.ItemIds[i]].ToString()}";

                resources.Add(resource);
            }
        }
    }

    private void CheckRequireItems()
    {
        for (int i = 0; i < resources.Count; ++i)
        {

        }
    }
}
