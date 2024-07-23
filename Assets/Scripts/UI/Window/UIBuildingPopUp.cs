using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildingPopUp : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.BUILDING_POPUP;

    public VillageManager vm;
    public UIManager um;

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

    private void Awake()
    {
        vm = GameManager.villageManager;
        um = GameManager.uiManager;
    }

    private void OnEnable()
    {
        SetText();
        SetRequireItem();

        if (!checkRequireItem())
            upgrade.interactable = false;
        else
            upgrade.interactable = true;

        vm.village.upgrade = um.currentNormalBuidling.gameObject.GetComponent<BuildingUpgrade>();
    }

    public void OnButtonUpgrade()
    {
        vm.village.Upgrade();
    }

    public void OnButtonExit()
    {
        vm.village.Cancel();
        gameObject.SetActive(false);
    }

    public void SetText()
    {
        buildingName.text = um.currentNormalBuidling.StructureName;
        defaultDescription.text = um.currentNormalBuidling.StructureDesc;
        currentEffectDescription.text = um.currentNormalBuidling.gameObject.GetComponent<BuildingUpgrade>().UpgradeDesc;
        //nextEffectDescription.text = ;
    }

    public void SetRequireItem()
    {
        for(int i = 0; i < resourceList.Count; ++i)
        {
            Destroy(resourceList[i].gameObject);
        }
        resourceList.Clear();

        for(int i = 0; i < kindOfResource; ++i)
        {
            var resource = Instantiate(upgradeResource, resourceLayout);
            //resource.GetComponent<TextMeshProUGUI>().text = ;
            //resource.GetComponent<Image>().sprite = ;

            resourceList.Add(resource);
        }
    }

    public bool checkRequireItem()
    {
        for(int i = 0; i < kindOfResource; ++i)
        {
            if(true)
            {
                ColorBlock colorBlock = upgrade.colors;
                colorBlock.normalColor = Color.green;
                upgrade.colors = colorBlock;
                resourceList[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
            }
            else
            {
                ColorBlock colorBlock = upgrade.colors;
                colorBlock.normalColor = Color.gray;
                upgrade.colors = colorBlock;
                resourceList[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
            }
        }

        foreach(var resource in resourceList)
        {
            if (resource.GetComponentInChildren<TextMeshProUGUI>().color == Color.red)
                return false;
        }
        return true;
    }
}
