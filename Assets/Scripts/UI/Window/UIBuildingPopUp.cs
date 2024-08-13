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

        if (!CheckRequireItem())
            upgrade.interactable = false;
        else
            upgrade.interactable = true;
    }

    public void OnButtonUpgrade()
    {
        vm.village.upgrade = upgradeComponent;
        vm.village.Upgrade();

        for (int i = 0; i < requireItemIds.Count; ++i)
        {
            im.ownItemList[requireItemIds[i]] -= requireItemNums[i];
        }

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

        for (int i = 0; i < requireItemIds.Count; ++i)
        {
            var resource = Instantiate(upgradeResource, resourceLayout);
            resource.GetComponentInChildren<TextMeshProUGUI>().text = $"{im.ownItemList[requireItemIds[i]]} / {requireItemNums[i]}";

            //var fileName = DataTableManager.itemTable.GetData(requireItemIds[i]).CurrencyAssetFileName;
            //resource.GetComponent<Image>().sprite = ;

            resourceList.Add(resource);
        }
    }

    public bool CheckRequireItem()
    {
        bool check = true;
        
        for (int i = 0; i < requireItemIds.Count; ++i)
        {
            if (requireItemNums[i] <= im.ownItemList[requireItemIds[i]])
            {
                upgrade.targetGraphic.color = Color.green;
                resourceList[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
            }
            else
            {
                upgrade.targetGraphic.color = Color.gray;
                resourceList[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
                check = false;
            }
        }

        if (GameManager.playerManager.level < upgradeComponent.RequirePlayerLv)
            return false;

        if (upgradeComponent.UpgradeGrade >= grade.Count)
            return false;

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
