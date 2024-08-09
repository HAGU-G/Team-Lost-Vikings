using System.Collections.Generic;
using TMPro;
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


            requireItemIds = grade[upgradeComponent.UpgradeGrade].ItemIds;
            requireItemNums = grade[upgradeComponent.UpgradeGrade].ItemNums;
            SetPopUp();
        }
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
        im.Gold -= UpgradeData.GetUpgradeData(upgradeComponent.UpgradeId, upgradeComponent.UpgradeGrade).RequireGold;
        //TO-DO : 요구 아이템 줄어들도록 수정하기
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

        for(int i = 0; i < units.Count; ++i)
        {
            var character = Instantiate(characterInformation, characterContent);
            var info = character.GetComponent<CharacterInfo>();
            info.parameterBar.interactable = false;
            info.characterId = units[i].stats.InstanceID;
            //info.gradeIcon.sprite = ;
            //info.characterGrade.text = units[i].stats.UnitGrade.ToString(); //없어질 예정
            info.characterName.text = units[i].stats.Data.Name;
            info.characterIcon.uvRect
                = GameManager.uiManager.unitRenderTexture.LoadRenderTexture(units[i].stats.Data.UnitAssetFileName);

            characters.Add(character);
        }
    }

    private void Update()
    {
        //CheckCurrentBuilding();
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

        var requireGold = grade[upgradeComponent.UpgradeGrade].RequireGold;
        var resource = Instantiate(upgradeResource, resourceLayout);
        resource.GetComponentInChildren<TextMeshProUGUI>().text = $"{im.Gold} / {requireGold.ToString()}";
        resourceList.Add(resource);

        //var requireItemIds = grade[upgradeComponent.UpgradeGrade].ItemIds;
        //var requireItemNums = grade[upgradeComponent.UpgradeGrade].ItemNums;

        //for (int i = 0; i < kindOfResource; ++i)
        //{
        //    var resource = Instantiate(upgradeResource, resourceLayout);

        //    //TO-DO : 소유 중인 아이템 / 테이블에서 요구하는 아이템 스트링 테이블 연결값
        //    resource.GetComponentInChildren<TextMeshProUGUI>().text = $"{im.ownItemList.GetValueOrDefault(requireItemIds[i])} / {requireItemNums[i]}";
        //    //resource.GetComponent<Image>().sprite = ;

        //    resourceList.Add(resource);
        //}
    }

    public bool checkRequireItem()
    {
        if (upgradeComponent.UpgradeGrade >= grade.Count)
            return false;

        if (im.Gold < grade[upgradeComponent.UpgradeGrade].RequireGold
                && im.Rune < grade[upgradeComponent.UpgradeGrade].RequireRune)
            return false;


        var requireGold = grade[upgradeComponent.UpgradeGrade].RequireGold;
        if(requireGold <= im.Gold)
        {
            ColorBlock colorBlock = upgrade.colors;
            colorBlock.normalColor = Color.green;
            colorBlock.pressedColor = Color.green;
            upgrade.colors = colorBlock;
            resourceList[0].GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
        }
        else
        {
            ColorBlock colorBlock = upgrade.colors;
            colorBlock.normalColor = Color.gray;
            colorBlock.pressedColor = Color.gray;
            upgrade.colors = colorBlock;
            resourceList[0].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
        }

        //var requireItemIds = grade[upgradeComponent.UpgradeGrade].ItemIds;
        //var requireItemNums = grade[upgradeComponent.UpgradeGrade].ItemNums;

        //for (int i = 0; i < kindOfResource; ++i)
        //{
        //    //requireItemNums[i] <= im.ownItemList.GetValueOrDefault(i)
        //    if (requireItemNums[i] <= im.ownItemList.GetValueOrDefault(i))
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
