using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildingPopUp : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.BUILDING_POPUP;

    public VillageManager vm;
    public UIManager um;
    public ItemManager im;

    public Button upgrade;
    public Button exit;

    public TextMeshProUGUI buildingName;
    public TextMeshProUGUI defaultDescription;
    public TextMeshProUGUI currentEffectDescription;
    public TextMeshProUGUI nextEffectDescription;

    public GameObject upgradeResource;
    public int kindOfResource = 5;
    public Transform resourceLayout;
    public List<GameObject> resourceList;

    public BuildingUpgrade upgradeComponent;
    public List<UpgradeData> grade;
    public List<int> requireItemIds;
    public List<int> requireItemNums;


    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnGameStart()
    {
        base.OnGameStart();
        vm = GameManager.villageManager;
        um = GameManager.uiManager;
        im = GameManager.itemManager;
    }


    private void OnEnable()
    {
        if (!IsReady)
            return;

        upgradeComponent = um.currentNormalBuidling.gameObject.GetComponent<BuildingUpgrade>();
        grade = DataTableManager.upgradeTable.GetData(um.currentNormalBuidling.UpgradeId);

        if (upgradeComponent.UpgradeGrade >= grade.Count)
        {
            SetLastUpgrade();
            return;
        }

        requireItemIds = grade[upgradeComponent.UpgradeGrade].ItemIds;
        requireItemNums = grade[upgradeComponent.UpgradeGrade].ItemNums;
        vm.village.upgrade = um.currentNormalBuidling.gameObject.GetComponent<BuildingUpgrade>();
        SetPopUp();
    }

    private void SetPopUp()
    {
        SetText();
        SetRequireItem();

        if (!CheckRequireItem())
            upgrade.interactable = false;
        else
            upgrade.interactable = true;
    }

    public void OnButtonUpgrade()
    {
        vm.village.upgrade = upgradeComponent;
        vm.village.Upgrade();
        im.Gold -= UpgradeData.GetUpgradeData(upgradeComponent.UpgradeId, upgradeComponent.UpgradeGrade).RequireGold;

        //for (int i = 0; i < kindOfResource; ++i)
        //{
        //    im.ownItemList[i] -= requireItemNums[i];
        //}
        SetPopUp();
    }

    public void OnButtonExit()
    {
        vm.village.Cancel();
        Close();
    }

    public void SetText()
    {
        buildingName.text = um.currentNormalBuidling.StructureName;
        defaultDescription.text = um.currentNormalBuidling.StructureDesc;
        currentEffectDescription.text = UpgradeData.GetUpgradeData(upgradeComponent.UpgradeId, upgradeComponent.UpgradeGrade).UpgradeDesc;
        
        if (upgradeComponent.UpgradeGrade < grade.Count)
            nextEffectDescription.text = UpgradeData.GetUpgradeData(upgradeComponent.UpgradeId, upgradeComponent.UpgradeGrade + 1).UpgradeDesc;
        else
        {
            nextEffectDescription.text = $"현재 마지막 업그레이드 단계입니다.";
        }
    }

    public void SetRequireItem()
    {
        for (int i = 0; i < resourceList.Count; ++i)
        {
            Destroy(resourceList[i].gameObject);
        }
        resourceList.Clear();

        if (upgradeComponent.UpgradeGrade >= grade.Count)
            return;

        var requireGold = grade[upgradeComponent.UpgradeGrade].RequireGold;
        var resource = Instantiate(upgradeResource, resourceLayout);
        resource.GetComponentInChildren<TextMeshProUGUI>().text = $"{im.Gold} / {requireGold.ToString()}";
        resourceList.Add(resource);

        //for (int i = 0; i < requireItemIds.Count; ++i)
        //{
        //    var resource = Instantiate(upgradeResource, resourceLayout);
        //    resource.GetComponentInChildren<TextMeshProUGUI>().text = $"{im.ownItemList.GetValueOrDefault(i)} / {requireItemIds[i]}";
        //    resource.GetComponentInChildren<Image>().sprite = ;

        //    resourceList.Add(resource);
        //}
    }

    public bool CheckRequireItem()
    {
        bool check = true;
        var requireGold = grade[upgradeComponent.UpgradeGrade].RequireGold;
        if (requireGold <= im.Gold)
        {
            upgrade.targetGraphic.color = Color.green;
            //ColorBlock colorBlock = upgrade.colors;
            //colorBlock.normalColor = Color.green;
            //upgrade.colors = colorBlock;
            resourceList[0].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
        }
        else
        {
            upgrade.targetGraphic.color = Color.gray;
            //ColorBlock colorBlock = upgrade.colors;
            //colorBlock.normalColor = Color.gray;
            //upgrade.colors = colorBlock;
            resourceList[0].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
            check = false;
        }

        //플레이어 레벨 검사후 업그레이드 활성/비활성화
        //if(GameManager.playerManager.level < upgradeComponent.RequirePlayerLv)
        //    return false;

        if (upgradeComponent.UpgradeGrade >= grade.Count)
            return false;

        //for (int i = 0; i < requireItemIds.Count; ++i)
        //{
        //    if (requireItemNums[i] <= im.ownItemList.GetValueOrDefault(i))
        //    {
        //        upgrade.targetGraphic.color = Color.green;
        //        //ColorBlock colorBlock = upgrade.colors;
        //        //colorBlock.normalColor = Color.green;
        //        //upgrade.colors = colorBlock;
        //        resourceList[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
        //    }
        //    else
        //    {
        //        upgrade.targetGraphic.color = Color.gray;
        //        //ColorBlock colorBlock = upgrade.colors;
        //        //colorBlock.normalColor = Color.gray;
        //        //upgrade.colors = colorBlock;
        //        resourceList[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
        //        check = false;
        //    }
        //}
        return check;
    }

    public void SetLastUpgrade()
    {

        SetText();

        for (int i = 0; i < resourceList.Count; ++i)
        {
            Destroy(resourceList[i].gameObject);
        }
        resourceList.Clear();

        upgrade.interactable = false;

    }
}
