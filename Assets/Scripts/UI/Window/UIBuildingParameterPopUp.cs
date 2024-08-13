using System.Collections.Generic;
using TMPro;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildingParameterPopUp : UIWindow
{
    public VillageManager vm;
    public UIManager um;
    public ItemManager im;

    public override WINDOW_NAME WindowName => WINDOW_NAME.PARAMETER_POPUP;

    public Button upgrade;
    public Button exit;

    public TextMeshProUGUI buildingName;
    public TextMeshProUGUI defaultDescription;
    public TextMeshProUGUI nextEffectDescription;

    public GameObject characterInformation;
    public Transform characterContent;
    public List<GameObject> characters;

    public GameObject upgradeResource;
    public int kindOfResource = 5;
    public Transform resourceLayout;
    public List<GameObject> resourceList;

    public BuildingUpgrade upgradeComponent;
    public List<UpgradeData> grade;
    public List<int> requireItemIds;
    public List<int> requireItemNums;

    private bool isOpen = false;

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
    }

    private void OnGameConfigure()
    {
        im.OnItemChangedCallback += OnItemChanged;
    }

    private void OnEnable()
    {
        if (!IsReady)
            return;

        isOpen = true;

        vm.village.upgrade = um.currentNormalBuidling.gameObject.GetComponent<BuildingUpgrade>();
        upgradeComponent = um.currentNormalBuidling.gameObject.GetComponent<BuildingUpgrade>();
        grade = DataTableManager.upgradeTable.GetData(um.currentNormalBuidling.UpgradeId);

        if (upgradeComponent.UpgradeGrade >= grade.Count)
        {
            SetLastUpgrade();
            return;
        }


        requireItemIds = grade[upgradeComponent.UpgradeGrade - 1].ItemIds;
        requireItemNums = grade[upgradeComponent.UpgradeGrade - 1].ItemNums;
        SetPopUp();
    }


    private void CheckCurrentBuilding()
    {
        Debug.Log($"upgrade : {vm.village.upgrade} / current : {um.currentParameterBuilding}");
        if (vm.village.upgrade != um.currentNormalBuidling.gameObject.GetComponent<BuildingUpgrade>())
        {
            vm.village.upgrade = um.currentNormalBuidling.gameObject.GetComponent<BuildingUpgrade>();
            upgradeComponent = um.currentNormalBuidling.gameObject.GetComponent<BuildingUpgrade>();
            grade = DataTableManager.upgradeTable.GetData(um.currentNormalBuidling.UpgradeId);

            if (upgradeComponent.UpgradeGrade >= grade.Count)
            {
                SetLastUpgrade();
                return;
            }

            requireItemIds = grade[upgradeComponent.UpgradeGrade - 1].ItemIds;
            requireItemNums = grade[upgradeComponent.UpgradeGrade - 1].ItemNums;
            SetPopUp();
        }
    }


    private void SetPopUp()
    {
        SetText();
        SetRequireItem();
        SetCharacterInformation();

        if (!CheckRequireItem())
            upgrade.interactable = false;
        else
            upgrade.interactable = true;
    }

    public void OnButtonUpgrade()
    {
        vm.village.Upgrade();
        for (int i = 0; i < requireItemIds.Count; ++i)
        {
            im.SpendItem(requireItemIds[i], requireItemNums[i]);
        }

        //업적 카운팅
        var buildingID = upgradeComponent.GetComponent<Building>().StructureId;
        GameManager.questManager.SetAchievementCountByTargetID(buildingID, ACHIEVEMENT_TYPE.BUILDING_UPGRADE, 1);
        
        if (upgradeComponent.UpgradeGrade >= grade.Count)
        {
            SetLastUpgrade();
            return;
        }

        SetPopUp();
    }

    public void OnButtonExit()
    {
        isOpen = false;
        Close();
    }

    public void SetText()
    {
        buildingName.text = um.currentNormalBuidling.StructureName;
        defaultDescription.text = um.currentNormalBuidling.StructureDesc;
        if (upgradeComponent.UpgradeGrade < grade.Count)
            //nextEffectDescription.text = UpgradeData.GetUpgradeData(upgradeComponent.UpgradeId, upgradeComponent.UpgradeGrade + 1).UpgradeDesc;
            //프로토타입까지는 다음 레벨 효과가 아닌 현재 레벨 효과로 글자 출력
            nextEffectDescription.text = UpgradeData.GetUpgradeData(upgradeComponent.UpgradeId, upgradeComponent.UpgradeGrade).UpgradeDesc;
        else
        {
            nextEffectDescription.text = $"현재 마지막 업그레이드 단계입니다.";
        }
    }

    public void SetCharacterInformation()
    {
        if (vm == null || vm.village.upgrade == null)
            return;
        for (int i = 0; i < characters.Count; ++i)
        {
            Destroy(characters[i].gameObject);
        }
        characters.Clear();

        var parameter = vm.village.upgrade.gameObject.GetComponent<ParameterRecoveryBuilding>();
        if (parameter == null)
            return;

        var units = parameter.interactingUnits;

        for (int i = 0; i < units.Count; ++i)
        {
            var character = Instantiate(characterInformation, characterContent);
            var info = character.GetComponent<CharacterInfo>();
            info.parameterBar.interactable = false;
            info.characterId = units[i].stats.InstanceID;
            info.characterName.text = units[i].stats.Data.Name;
            info.characterIcon.uvRect
                = GameManager.uiManager.unitRenderTexture.LoadRenderTexture(units[i].stats.Data.UnitAssetFileName);

            characters.Add(character);
        }
    }

    private void Update()
    {
        SetParameterBar();
    }

    public void SetParameterBar()
    {
        if (vm.village.upgrade == null)
            return;

        var parameter = vm.village.upgrade.gameObject.GetComponent<ParameterRecoveryBuilding>();
        if (parameter == null)
            return;

        foreach (var character in characters)
        {
            var info = character.GetComponent<CharacterInfo>();
            var unit = GameManager.unitManager.GetUnit(info.characterId);
            switch (parameter.parameterType)
            {
                case PARAMETER_TYPE.HP:
                    info.parameterBar.value = (float)unit.HP.Current / (float)unit.HP.max;
                    break;
                case PARAMETER_TYPE.STAMINA:
                    info.parameterBar.value = (float)unit.Stamina.Current / (float)unit.Stamina.max;
                    break;
                case PARAMETER_TYPE.MENTAL:
                    info.parameterBar.value = (float)unit.Stress.Current / (float)unit.Stress.max;
                    break;
            }
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
            resource.GetComponentInChildren<TextMeshProUGUI>().text = $"{im.GetItem(requireItemIds[i])} / {requireItemNums[i]}";

            //var fileName = DataTableManager.itemTable.GetData(requireItemIds[i]).CurrencyAssetFileName;
            //resource.GetComponent<Image>().sprite = ;

            resourceList.Add(resource);
        }
    }

    private void SetResourceText()
    {
        for (int i = 0; i < resourceList.Count; ++i)
        {
            resourceList[i].GetComponentInChildren<TextMeshProUGUI>().text = $"{im.GetItem(requireItemIds[i])} / {requireItemNums[i]}";
        }

        if (!CheckRequireItem())
            upgrade.interactable = false;
        else
            upgrade.interactable = true;
    }

    private void OnItemChanged()
    {
        if (isOpen)
            SetResourceText();
    }

    public bool CheckRequireItem()
    {
        bool check = true;

        for (int i = 0; i < requireItemIds.Count; ++i)
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
