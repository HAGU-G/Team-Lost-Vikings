using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class UIReviveBuilding : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.REVIVE_POPUP;

    public TextMeshProUGUI buildingName;
    public TextMeshProUGUI buildingDesc;

    public Transform reviveTransform;
    public GameObject revivePrefab;
    private List<GameObject> reviveList = new();

    public TextMeshProUGUI reviveTimeText;

    public Transform upgradeTransform;
    public GameObject upgradePrefab;
    private List<GameObject> resourceList = new();

    public Button upgrade;
    public Button exit;

    private UIManager um;
    private ItemManager im;
    private VillageManager vm;

    private BuildingUpgrade upgradeComponent;
    private List<UpgradeData> grade;

    private List<int> requireItemIds = new();
    private List<int> requireItemNums = new();

    private bool isOpen = false;
    private Dictionary<int, Sprite> itemIcons = new();

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnGameStart()
    {
        base.OnGameStart();
        um = GameManager.uiManager;
        im = GameManager.itemManager;
        vm = GameManager.villageManager;

        upgrade.onClick.AddListener(OnButtonUpgrade);
        exit.onClick.AddListener(OnButtonExit);

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
        upgradeComponent = um.currentNormalBuidling.gameObject.GetComponent<BuildingUpgrade>();
        grade = DataTableManager.upgradeTable.GetData(um.currentNormalBuidling.UpgradeId);
        vm.village.upgrade = upgradeComponent;

        isOpen = true;

        if (upgradeComponent.UpgradeGrade >= grade.Count)
        {
            SetLastUpgrade();
            return;
        }

        requireItemIds = grade[upgradeComponent.UpgradeGrade].ItemIds;
        requireItemNums = grade[upgradeComponent.UpgradeGrade].ItemNums;

        SetUI();
    }

    private void OnItemChanged()
    {
        if (isOpen)
            SetResourceText();
    }

    private void SetUI()
    {
        buildingName.text = um.currentNormalBuidling.StructureName;
        buildingDesc.text = um.currentNormalBuidling.StructureDesc;

        SetRevivingList();

        if (upgradeComponent.currentGrade >= grade.Count)
        {
            upgrade.interactable = false;
        }

        var time = um.currentNormalBuidling.gameObject.GetComponent<ReviveBuilding>().reviveTime;
        //reviveTimeText.text = $"부활 대기시간 {time}초";

        if (upgradeComponent.UpgradeGrade < grade.Count)
        {
            reviveTimeText.text = $"{grade[upgradeComponent.UpgradeGrade].UpgradeDesc}";
            SetUpgradeItemList();
            CheckResource();
        }
    }

    public void SetRevivingList()
    {
        for (int i = reviveList.Count - 1; i >= 0; i--)
        {
            Destroy(reviveList[i]);
        }
        reviveList.Clear();

        var reviveBuilding = GameManager.villageManager.GetBuilding(STRUCTURE_ID.REVIVE);
        var reviveComponent = reviveBuilding.GetComponent<ReviveBuilding>();
        foreach (var unit in reviveComponent.revivingUnits)
        {
            var obj = Instantiate(revivePrefab, reviveTransform);
            var info = obj.GetComponent<CharacterInfo>();
            info.characterName.text = unit.stats.Data.Name;
            info.characterIcon.uvRect = GameManager.uiManager.unitRenderTexture.LoadRenderTexture(unit.stats.Data.UnitAssetFileName);
            info.parameterBar.interactable = false;
            info.characterId = unit.stats.InstanceID;

            reviveList.Add(obj);
        }
    }

    private void SetUpgradeItemList()
    {
        foreach (var resource in resourceList)
        {
            Destroy(resource);
        }
        resourceList.Clear();

        if (upgradeComponent.UpgradeGrade >= DataTableManager.upgradeTable.GetData(upgradeComponent.UpgradeId).Count)
            return;

        requireItemIds = grade[upgradeComponent.UpgradeGrade].ItemIds;
        requireItemNums = grade[upgradeComponent.UpgradeGrade].ItemNums;

        for (int i = 0; i < requireItemIds.Count; ++i)
        {
            var resource = Instantiate(upgradePrefab, upgradeTransform);
            resource.GetComponentInChildren<TextMeshProUGUI>().text = $"{im.GetItem(requireItemIds[i])} / {requireItemNums[i]}";

            var fileName = DataTableManager.upgradeTable.GetData(upgradeComponent.UpgradeId)[upgradeComponent.UpgradeGrade].ItemIds[i];
            var exist = itemIcons.TryGetValue(fileName, out var value);
            resource.GetComponentInChildren<Image>().sprite = value;

            resourceList.Add(resource);

            if (value == null)
            {
                resource.SetActive(false);
            }
        }

        //CheckResource();
    }

    private bool CheckResource()
    {
        bool isEnough = true;

        for (int i = 0; i < resourceList.Count; ++i)
        {
            var upgradeData = grade[upgradeComponent.UpgradeGrade];
            if (upgradeData.ItemNums[i] <= im.GetItem(requireItemIds[i]))
            {
                resourceList[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
            }
            else
            {
                resourceList[i].GetComponentInChildren<TextMeshProUGUI>().color = new Color(255f / 255f, 8f / 255f, 0f / 255f);
                isEnough = false;
            }
        }
        if (upgradeComponent.UpgradeGrade >= grade.Count)
            isEnough = false;

        if (GameManager.playerManager.level < grade[upgradeComponent.UpgradeGrade].RequirePlayerLv)
            isEnough = false;

        upgrade.interactable = isEnough;
        return isEnough;
    }

    public void SetProgressBar(float timer, float reviveTime)
    {
        //CharacterInfo의 id를 참조하여 timer 값을 불러오기 때문에
        //timer를 사용하지 않게 됨.

        if (vm.village.upgrade == null)
            return;

        var revive = vm.village.upgrade.gameObject.GetComponent<ReviveBuilding>();
        if (revive == null)
            return;

        for (int i = reviveList.Count - 1; i >= 0; i--)
        {
            var character = reviveList[i];
            var info = character.GetComponent<CharacterInfo>();
            var unit = GameManager.unitManager.GetUnit(info.characterId);

            if (unit != null)
                info.parameterBar.value = unit.reviveTimer / reviveTime;

            if (unit == null || (unit.reviveTimer / reviveTime >= 1f))
            {
                Destroy(character);
                reviveList.RemoveAt(i);
            }
        }


    }

    public void OnButtonUpgrade()
    {
        GameManager.PlayButtonSFX();


        requireItemIds = grade[upgradeComponent.UpgradeGrade].ItemIds;
        requireItemNums = grade[upgradeComponent.UpgradeGrade].ItemNums;
        for (int i = 0; i < requireItemIds.Count; ++i)
        {
            im.SpendItem(requireItemIds[i], requireItemNums[i]);
        }

        vm.village.Upgrade();
        
        if (upgradeComponent.UpgradeGrade >= grade.Count)
        {
            SetLastUpgrade();
            return;
        }
        SetUI();
    }

    public void OnButtonExit()
    {
        GameManager.PlayButtonSFX();
        isOpen = false;
        Close();
    }

    private void SetResourceText()
    {
        for (int i = 0; i < resourceList.Count; ++i)
        {
            resourceList[i].GetComponentInChildren<TextMeshProUGUI>().text = $"{im.GetItem(requireItemIds[i])} / {requireItemNums[i]}";
        }

        if (!CheckResource())
            upgrade.interactable = false;
        else
            upgrade.interactable = true;
    }

    public void SetLastUpgrade()
    {
        SetUI();

        for (int i = 0; i < resourceList.Count; ++i)
        {
            Destroy(resourceList[i].gameObject);
        }
        resourceList.Clear();
        var time = um.currentNormalBuidling.gameObject.GetComponent<ReviveBuilding>().reviveTime;
        reviveTimeText.text = $"마지막 업그레이드 단계입니다.";
        upgrade.interactable = false;
    }
}