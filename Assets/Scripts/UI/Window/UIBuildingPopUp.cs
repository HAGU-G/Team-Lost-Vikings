﻿using System.Collections.Generic;
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

        if (!checkRequireItem())
            upgrade.interactable = false;
        else
            upgrade.interactable = true;
    }

    public void OnButtonUpgrade()
    {
        vm.village.upgrade = upgradeComponent;
        vm.village.Upgrade();
        im.Gold -= grade[upgradeComponent.UpgradeGrade].RequireGold;
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
        currentEffectDescription.text = um.currentNormalBuidling.gameObject.GetComponent<BuildingUpgrade>().UpgradeDesc;
        if (upgradeComponent.UpgradeGrade < grade.Count)
            nextEffectDescription.text = UpgradeData.GetUpgradeData(upgradeComponent.UpgradeId, upgradeComponent.UpgradeGrade + 1).UpgradeDesc;
        else
        {
            nextEffectDescription.text = $"현재 마지막 업그레이드 단계입니다.";
        }
    }

    public void SetRequireItem()
    {
        for(int i = 0; i < resourceList.Count; ++i)
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

        //for (int i = 0; i < kindOfResource; ++i)
        //{
        //    var resource = Instantiate(upgradeResource, resourceLayout);
        //    resource.GetComponentInChildren<TextMeshProUGUI>().text = $"{im.ownItemList.GetValueOrDefault(i)} / {requireItemIds[i]}";
        //    //resource.GetComponent<Image>().sprite = ;

        //    resourceList.Add(resource);
        //}
    }

    public bool checkRequireItem()
    {
        if (upgradeComponent.UpgradeGrade >= grade.Count)
            return false;

        var requireGold = grade[upgradeComponent.UpgradeGrade].RequireGold;
        if (requireGold <= im.Gold)
        {
            ColorBlock colorBlock = upgrade.colors;
            colorBlock.normalColor = Color.green;
            upgrade.colors = colorBlock;
            resourceList[0].GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
        }
        else
        {
            ColorBlock colorBlock = upgrade.colors;
            colorBlock.normalColor = Color.gray;
            upgrade.colors = colorBlock;
            resourceList[0].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
        }

        //for (int i = 0; i < kindOfResource; ++i)
        //{
        //    if(im.ownItemList.GetValueOrDefault(i) >= requireItemNums[i])
        //    {
        //        ColorBlock colorBlock = upgrade.colors;
        //        colorBlock.normalColor = Color.green;
        //        upgrade.colors = colorBlock;
        //        resourceList[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
        //    }
        //    else
        //    {
        //        ColorBlock colorBlock = upgrade.colors;
        //        colorBlock.normalColor = Color.gray;
        //        upgrade.colors = colorBlock;
        //        resourceList[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
        //    }
        //}

        foreach(var resource in resourceList)
        {
            if (resource.GetComponentInChildren<TextMeshProUGUI>().color == Color.red)
                return false;
        }
        return true;
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
