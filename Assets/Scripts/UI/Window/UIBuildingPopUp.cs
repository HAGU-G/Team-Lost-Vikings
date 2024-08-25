using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
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

    private bool isOpen = false;

    private Dictionary<int, Sprite> itemIcons = new();

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

        GameManager.Subscribe(EVENT_TYPE.CONFIGURE, OnGameConfigure);

        var path = "Assets/Scenes/Design/Icon/";
        var itemDatas = DataTableManager.itemTable.GetDatas();
        for (int i = 0; i < itemDatas.Count; ++i)
        {
            var newPath = $"{path}{itemDatas[i].CurrencyAssetFileName}.png";
            var id = itemDatas[i].CurrencyId;
            Addressables.LoadAssetAsync<Sprite>(newPath).Completed += (obj) => OnLoadDone(obj, id);
        }
    }

    private void OnLoadDone(AsyncOperationHandle<Sprite> obj, int id)
    {
        itemIcons.Add(id, obj.Result);
    }

    private void OnGameConfigure()
    {
        im.OnItemChangedCallback += OnItemChanged;
    }


    private void OnEnable()
    {
        if (!IsReady)
            return;

        upgradeComponent = um.currentNormalBuidling.gameObject.GetComponent<BuildingUpgrade>();
        grade = DataTableManager.upgradeTable.GetData(um.currentNormalBuidling.UpgradeId);

        isOpen = true;

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

    private void OnItemChanged()
    {
        if (isOpen)
            SetResourceText();
    }

    private void SetPopUp()
    {
        SetText();
        SetRequireItem();

        upgrade.interactable = CheckRequireItem();
    }

    public void OnButtonUpgrade()
    {
        GameManager.PlayButtonSFX();
        vm.village.upgrade = upgradeComponent;
        
        requireItemIds = grade[upgradeComponent.UpgradeGrade].ItemIds;
        requireItemNums = grade[upgradeComponent.UpgradeGrade].ItemNums;
        for (int i = 0; i < requireItemIds.Count; ++i)
        {
            im.SpendItem(requireItemIds[i], requireItemNums[i]);
        }

        //업적 카운팅
        var buildingID = upgradeComponent.GetComponent<Building>().StructureId;
        GameManager.questManager.SetAchievementCountByTargetID(buildingID, ACHIEVEMENT_TYPE.BUILDING_UPGRADE, 1);

        vm.village.Upgrade(); 
        Debug.Log(upgradeComponent.UpgradeGrade);
        if (upgradeComponent.UpgradeGrade >= grade.Count)
        {
            SetLastUpgrade();
            return;
        }
        SetPopUp();
    }

    public void OnButtonExit()
    {
        GameManager.PlayButtonSFX();
        isOpen = false;
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
        requireItemIds = grade[upgradeComponent.UpgradeGrade].ItemIds;
        requireItemNums = grade[upgradeComponent.UpgradeGrade].ItemNums;

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
            resource.GetComponentInChildren<TextMeshProUGUI>().text = $"{im.GetItem(requireItemIds[i])} / {requireItemNums[i]}";

            var fileName = DataTableManager.upgradeTable.GetData(upgradeComponent.UpgradeId)[upgradeComponent.UpgradeGrade].ItemIds[i];
            var exist = itemIcons.TryGetValue(fileName, out var value);
            resource.GetComponentInChildren<Image>().sprite = value;

            resourceList.Add(resource);

            if (value == null)
            {
                Destroy(resource);
                resourceList.Remove(resource);
            }
        }
    }

    public bool CheckRequireItem()
    {
        bool check = true;

        for (int i = 0; i < resourceList.Count; ++i)
        {
            if (requireItemNums[i] <= im.GetItem(requireItemIds[i]))
            {
                resourceList[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
            }
            else
            {
                resourceList[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
                check = false;
            }
        }

        if (upgradeComponent.UpgradeGrade >= grade.Count)
            return false;

        if (GameManager.playerManager.level < grade[upgradeComponent.UpgradeGrade].RequirePlayerLv)
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

        nextEffectDescription.text = "마지막 업그레이드 단계입니다.";

        upgrade.interactable = false;
    }

    private void SetResourceText()
    {
        for (int i = 0; i < resourceList.Count; ++i)
        {
            resourceList[i].GetComponentInChildren<TextMeshProUGUI>().text = $"{im.GetItem(requireItemIds[i])} / {requireItemNums[i]}";
        }

        upgrade.interactable = CheckRequireItem();
    }
}
