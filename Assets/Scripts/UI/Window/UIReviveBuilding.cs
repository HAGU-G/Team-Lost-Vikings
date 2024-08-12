using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TerrainUtils;
using UnityEngine.TextCore.Text;
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
    }

    private void OnEnable()
    {
        upgradeComponent = um.currentNormalBuidling.gameObject.GetComponent<BuildingUpgrade>();
        grade = DataTableManager.upgradeTable.GetData(um.currentNormalBuidling.UpgradeId);
        vm.village.upgrade = upgradeComponent;

        SetUI();
    }

    private void SetUI()
    {
        buildingName.text = um.currentNormalBuidling.StructureName;
        buildingDesc.text = um.currentNormalBuidling.StructureDesc;

        SetRevivingList();

        if(upgradeComponent.currentGrade >= grade.Count)
        {
            upgrade.interactable = false; 
            
        }

        var time = um.currentNormalBuidling.gameObject.GetComponent<ReviveBuilding>().reviveTime;
        reviveTimeText.text = $"부활 대기시간 {time}초";

        SetUpgradeItemList();
        CheckResource();
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
        foreach(var unit in reviveComponent.revivingUnits)
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
        foreach(var resource in resourceList)
        {
            Destroy(resource);
        }
        resourceList.Clear();

        var gold = Instantiate(upgradePrefab, upgradeTransform);
        gold.GetComponentInChildren<TextMeshProUGUI>().text = $"{im.Gold}/{upgradeComponent.RequireGold}";
        resourceList.Add(gold);

        //for (int i = 0; i < upgradeComponent.ItemIds.Count; ++i)
        //{
        //    var item = Instantiate(upgradePrefab, upgradeTransform);
        //    //item.GetComponentInChildren<TextMeshProUGUI>().text = $"{im.ownItemList[i]}/{upgradeComponent.ItemNums[i]}";
        //    //TO-DO : item 추가되면 주석 풀기

        //    //item.GetComponentInChildren<Image>().sprite = DataTableManager.
        //    //TO-DO : 재화 테이블 추가되면 수정하기
        //    resourceList.Add(item);
        //}

        //CheckResource();
    }

    private bool CheckResource()
    {
        bool isEnough = true;

        ///////////////////////////골드만 사용하는 임시 내용///////////////////
        if (im.Gold < upgradeComponent.RequireGold)
        {
            for(int i = 0; i < resourceList.Count; ++i)
            {
                upgrade.targetGraphic.color = Color.gray;
                resourceList[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
            }
            upgrade.interactable = false;
            return false;
        }

        for (int i = 0; i < resourceList.Count; ++i)
        {
            upgrade.targetGraphic.color = Color.green;
            resourceList[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
        }
        upgrade.interactable = isEnough;
        ////////////////////////////////////////////////////////////////////////////////////////


        //for (int i = 0; i < resourceList.Count; ++i)
        //{
        //    if (grade[upgradeComponent.UpgradeId].ItemNums[i] <= im.ownItemList[i])
        //    {
        //        resourceList[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
        //    }
        //    else
        //    {
        //        resourceList[i].GetComponentInChildren<TextMeshProUGUI>().color = new Color(255f / 255f, 8f / 255f, 0f / 255f);
        //        isEnough = false;
        //    }
        //}


        //if (upgradeComponent.RequirePlayerLv < GameManager.playerManager.level)
        //    return false;

        if (upgradeComponent.UpgradeGrade >= grade.Count)
            return false;


        return isEnough;
    }

    public void SetProgressBar(float timer, float reviveTime)
    {
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
            info.parameterBar.value = timer / reviveTime;

            if (timer / reviveTime >= 1f)
            {
                Destroy(character);
                reviveList.RemoveAt(i);
            }
        }


    }

    private void Update()
    {
    }

    public void OnButtonUpgrade()
    {
        if(im.Gold >= upgradeComponent.RequireGold)
        {
            im.Gold -= upgradeComponent.RequireGold;
            vm.village.Upgrade();
            SetUI();
        }
    }

    public void OnButtonExit()
    {
        Close();
    }
}