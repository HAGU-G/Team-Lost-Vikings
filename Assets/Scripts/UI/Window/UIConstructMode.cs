using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class UIConstructMode : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.CONSTRUCT_MODE;

    private ItemManager im;
    private UIManager um;

    public Transform content; // 건물 목록 스크롤뷰의 content
    public GameObject buidlingUIPrefab;
    public Button constructOff;
    public TextMeshProUGUI constructModeinProgress;

    public List<BuildingData> buildingDatas = DataTableManager.buildingTable.GetDatas();
    public List<UpgradeData> upgradeDatas = DataTableManager.upgradeTable.GetDatas();

    private Dictionary<BuildingData, GameObject> buildings = new();
    private Dictionary<Button, bool> buttonActivationStatus = new();

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnGameStart()
    {
        base.OnGameStart();
        im = GameManager.itemManager;
        um = GameManager.uiManager;
        MakeBuildingList();
        SortBuildingButtons();
    }

    private void OnEnable()
    {
        foreach(var building in buildingDatas)
        {
            CheckBuildingButton(building, buildings.GetValueOrDefault(building).GetComponentInChildren<Button>());
        }
        SortBuildingButtons();
    }

    private void MakeBuildingList()
    {
        foreach (var buildingData in buildingDatas)
        {
            var b = GameObject.Instantiate(buidlingUIPrefab, content);
            
            string assetName = buildingDatas[buildingData.UpgradeId].StructureAssetFileName;
            var path = $"{assetName}"; //TO-DO : 파일 경로 수정하기

            var handle = Addressables.LoadAssetAsync<GameObject>(path);
            handle.WaitForCompletion();

            b.GetComponentInChildren<Image>().sprite = handle.Result.GetComponentInChildren<SpriteRenderer>().sprite;
            var button = b.GetComponentInChildren<Button>();
            button.onClick.AddListener
            (() =>
            {
                um.currentBuildingData = buildingData;
                GameManager.uiManager.windows[WINDOW_NAME.BUILDING_DETAIL].Open();
            });
            CheckBuildingButton(buildingData, button);
            buildings.Add(buildingData, b);
        }
        
    }

    private void CheckBuildingButton(BuildingData data, Button button)
    {
        bool isActive = true;
        var upgradeData = upgradeDatas[data.UpgradeId];

        if (data.UnlockTownLevel > GameManager.playerManager.level)
        {
            SetButtonColor(button, false);
            isActive = false;
        }

        for (int i = 0; i < upgradeData.ItemIds.Count; ++i)
        {
            if (im.ownItemList[upgradeData.ItemIds[i]] < upgradeData.ItemNums[i])
            {
                SetButtonColor(button, false);
                isActive = false;
                break;
            }
            else
            {
                SetButtonColor(button, true);
                isActive = true;
            }
        }

        if (isActive)
            SetButtonColor(button, true);

        buttonActivationStatus[button] = isActive;
    }

    private void SetButtonColor(Button button, bool satisfy)
    {
        if (satisfy)
        {
            button.interactable = true;
            ColorBlock colorBlock = button.colors; 
            colorBlock.normalColor = new Color(200, 231, 167);
            //colorBlock.normalColor = Color.white;
            button.colors = colorBlock;
        }
        else
        {
            button.interactable = false;
            ColorBlock colorBlock = button.colors;
            colorBlock.normalColor = new Color(255, 128, 128);
            button.colors = colorBlock;
        }
    }

    private void SortBuildingButtons()
    {
        List<Transform> children = new();

        foreach(Transform child in content)
        {
            children.Add(child);
        }

        children.Sort((a, b) =>
        {
            var dataA = GetBuildingDataFromButton(a.GetComponentInChildren<Button>());
            var dataB = GetBuildingDataFromButton(b.GetComponentInChildren<Button>());
            bool isActiveA = buttonActivationStatus[a.GetComponentInChildren<Button>()];
            bool isActiveB = buttonActivationStatus[b.GetComponentInChildren<Button>()];

            if (isActiveA == isActiveB)
            {
                return dataA.UnlockTownLevel.CompareTo(dataB.UnlockTownLevel);
            }
            return isActiveA ? -1 : 1;
        });

        for (int i = 0; i < children.Count; ++i)
        {
            children[i].SetSiblingIndex(i);
        }
    }

    private BuildingData GetBuildingDataFromButton(Button button)
    {
        foreach (var kvp in buildings)
        {
            if (kvp.Value.GetComponentInChildren<Button>() == button)
            {
                return kvp.Key;
            }
        }
        return null;
    }
}
