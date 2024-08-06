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
    private VillageManager vm;
    private ConstructMode constructMode;

    public Transform content; // 건물 목록 스크롤뷰의 content
    public GameObject buidlingUIPrefab;

    public Button constructOff;
    public GameObject constructModeinProgress;

    public GameObject destroyPopUp;

    public List<BuildingData> buildingDatas = new();
    public List<UpgradeData> upgradeDatas = new();

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
        vm = GameManager.villageManager;
        constructMode = vm.constructMode;
        constructModeinProgress.SetActive(false);

        buildingDatas = DataTableManager.buildingTable.GetDatas();
        upgradeDatas = DataTableManager.upgradeTable.GetDatas();
    }

    public void OnButtonChangePlacement()
    {

    }

    public void OnButtonRotateBuilding()
    {
        um.currentNormalBuidling.RotateBuilding(um.currentNormalBuidling);
    }

    public void OnButtonDestroyBuilding()
    {
        destroyPopUp.SetActive(true);
    }

    public void OnButtonDestroyCommit()
    {
        constructMode.construct.RemoveBuilding(um.currentNormalBuidling, vm.gridMap);
    }

    public void OnButtonDestroyExit()
    {
        destroyPopUp.SetActive(false);
    }

    public void OnButtonExit()
    {
        constructModeinProgress.SetActive(false);
        GameManager.Publish(EVENT_TYPE.CONSTRUCT);
        Close();
    }

    private void OnEnable()
    {
        MakeBuildingList();
        constructModeinProgress.SetActive(true);

        GameManager.uiManager.uiDevelop.ConstructButtonsOff();
        destroyPopUp.SetActive(false);

        foreach (var building in buildingDatas)
        {
            if(buildings.TryGetValue(building, out GameObject value))
                CheckBuildingButton(building, value.GetComponent<Button>());
        }
        SortBuildingButtons();
    }

    private void MakeBuildingList()
    {
        if(buildings.Count != 0)
        {
            foreach(var building in buildings)
            {
                CheckBuildingButton(building.Key, building.Value.GetComponent<Button>());
            }
            return;
        }
        

        foreach (var buildingData in buildingDatas)
        {
            var b = GameObject.Instantiate(buidlingUIPrefab, content);
            
            string assetName = buildingData.StructureAssetFileName;
            var path = $"Assets/Pick_Asset/2WEEK/Building/{assetName}.prefab"; //TO-DO : 파일 경로 수정하기

            var handle = Addressables.LoadAssetAsync<GameObject>(path);
            handle.WaitForCompletion();

            b.GetComponent<Image>().sprite = handle.Result.GetComponentInChildren<SpriteRenderer>().sprite;
            var button = b.GetComponent<Button>();
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
        int grade;
        if (!GameManager.playerManager.buildingUpgradeGrades.TryGetValue(data.StructureId, out int value))
        {
            value = 1;
            grade = value;
        }
        else
        {
            grade = value;
        }

        if(data.UpgradeId == 0)
        {
            SetButtonColor(button, false);
            isActive = false;
            buttonActivationStatus[button] = isActive;
            return;
        }

        var upgradeData = DataTableManager.upgradeTable.GetData(data.UpgradeId)[grade];

        if (data.UnlockTownLevel > GameManager.playerManager.level)
        {
            SetButtonColor(button, false);
            isActive = false;
        }

        for (int i = 0; i < upgradeData.ItemIds.Count; ++i)
        {
            if (im.ownItemList.TryGetValue(upgradeData.ItemIds[i], out int itemNum))
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
            else
            {
                SetButtonColor(button, false);
                isActive = false;
                break;
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
            var dataA = GetBuildingDataFromButton(a.GetComponent<Button>());
            var dataB = GetBuildingDataFromButton(b.GetComponent<Button>());
            bool isActiveA = buttonActivationStatus[a.GetComponent<Button>()];
            bool isActiveB = buttonActivationStatus[b.GetComponent<Button>()];

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
            if (kvp.Value.GetComponent<Button>() == button)
            {
                return kvp.Key;
            }
        }
        return null;
    }
}
