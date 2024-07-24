using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

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

    public BuildingUpgrade upgradeComponent ;
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

        vm.village.upgrade = um.currentNormalBuidling.gameObject.GetComponent<BuildingUpgrade>(); 
        upgradeComponent = um.currentNormalBuidling.gameObject.GetComponent<BuildingUpgrade>();
        grade = DataTableManager.upgradeTable.GetData(um.currentNormalBuidling.UpgradeId);

        if (upgradeComponent.UpgradeGrade >= grade.Count)
        {
            SetLastUpgrade();
            return;
        }
            

        requireItemIds = grade[upgradeComponent.UpgradeGrade].ItemIds;
        requireItemNums = grade[upgradeComponent.UpgradeGrade].ItemNums;
        SetPopUp();
    }


    private void SetPopUp()
    {
        SetText();
        SetRequireItem();
        SetCharacterInformation();

        if (!checkRequireItem())
            upgrade.interactable = false;
        else
            upgrade.interactable = true;
    }

    public void OnButtonUpgrade()
    {
        vm.village.Upgrade();
        SetPopUp();
    }

    public void OnButtonExit()
    {
        Close();
    }

    public void SetText()
    {
        buildingName.text = um.currentNormalBuidling.StructureName;
        defaultDescription.text = um.currentNormalBuidling.StructureDesc;
        if (upgradeComponent.UpgradeGrade < grade.Count)
            nextEffectDescription.text = UpgradeData.GetUpgradeData(upgradeComponent.UpgradeId, upgradeComponent.UpgradeGrade + 1).UpgradeDesc;
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
        
        var units = parameter.interactingUnits;


        //Debug.Log(units.Count);
        for(int i = 0; i < units.Count; ++i)
        {
            var character = Instantiate(characterInformation, characterContent);
            var info = character.GetComponent<CharacterInfo>();
            info.characterId = units[i].stats.InstanceID;
            info.characterGrade.text = units[i].stats.UnitGrade.ToString();
            info.characterName.text = units[i].stats.Name;
            info.characterIcon.uvRect
                = GameManager.uiManager.unitRenderTexture.LoadRenderTexture(units[i].stats.AssetFileName);

            characters.Add(character);
        }
    }

    private void Update()
    {
        SetParameterBar();
    }

    public void SetParameterBar()
    {
        var parameter = vm.village.upgrade.gameObject.GetComponent<ParameterRecoveryBuilding>();

        foreach (var character in characters)
        {
            var info = character.GetComponent<CharacterInfo>();
            var unit = GameManager.unitManager.GetUnit(info.characterId);
            switch(parameter.parameterType)
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

        var requireItemIds = grade[upgradeComponent.UpgradeGrade].ItemIds;
        var requireItemNums = grade[upgradeComponent.UpgradeGrade].ItemNums;

        for (int i = 0; i < kindOfResource; ++i)
        {
            var resource = Instantiate(upgradeResource, resourceLayout);

            //TO-DO : 소유 중인 아이템 / 테이블에서 요구하는 아이템 스트링 테이블 연결값
            resource.GetComponentInChildren<TextMeshProUGUI>().text = $"{im.ownItemList.GetValueOrDefault(requireItemIds[i])} / {requireItemNums[i]}";
            //resource.GetComponent<Image>().sprite = ;

            resourceList.Add(resource);
        }
    }

    public bool checkRequireItem()
    {
        if (upgradeComponent.UpgradeGrade >= grade.Count)
            return false;

        if (im.Gold < grade[upgradeComponent.UpgradeGrade].RequireGold
                && im.Rune < grade[upgradeComponent.UpgradeGrade].RequireRune)
            return false;

        var requireItemIds = grade[upgradeComponent.UpgradeGrade].ItemIds;
        var requireItemNums = grade[upgradeComponent.UpgradeGrade].ItemNums;

        for (int i = 0; i < kindOfResource; ++i)
        {
            if (requireItemNums[i] <= im.ownItemList.GetValueOrDefault(i))
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

        foreach (var resource in resourceList)
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
