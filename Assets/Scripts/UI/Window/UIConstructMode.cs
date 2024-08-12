using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

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

    private UIBuildingDetail buildingDetail;
    private bool isReplacing = false;

    private List<UnitOnVillage> prevMovingUnits = new();
    private List<UnitOnVillage> prevInteractingUnits = new();
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

        buildingDetail = um.windows[WINDOW_NAME.BUILDING_DETAIL] as UIBuildingDetail;
    }

    public void OnButtonChangePlacement()
    {
        isReplacing = true;
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
        destroyPopUp.SetActive(false);

        ResetBuildingEffection(um.currentNormalBuidling);
        
        //TO-DO : 아이템 추가 후 재화 돌려받는 내용 적기

        foreach (var buildingData in buildingDatas)
        {
            if (buildings.TryGetValue(buildingData, out GameObject buildingObj))
            {
                CheckBuildingButton(buildingData, buildingObj);
            }
        }

        SortBuildingButtons();
        um.uiDevelop.ConstructButtonsOff();
    }

    public void OnButtonDestroyExit()
    {
        destroyPopUp.SetActive(false);
    }

    public void OnButtonExit()
    {
        FinishConstructMode();
    }

    public void FinishConstructMode()
    {
        GameManager.villageManager.constructMode.isConstructMode = false;
        constructModeinProgress.SetActive(false);
        Close();
    }

    public void EscapeConstructMode()
    {
        //다른 ui를 띄우는 경우

        //사냥터로 이동하는 경우
    }

    private void OnEnable()
    {
        GameManager.villageManager.constructMode.isConstructMode = true;
        MakeBuildingList();
        constructModeinProgress.SetActive(true);

        GameManager.uiManager.uiDevelop.ConstructButtonsOff();
        destroyPopUp.SetActive(false);

        foreach (var building in buildingDatas)
        {
            if (buildings.TryGetValue(building, out GameObject value))
                CheckBuildingButton(building, value);
        }
        SortBuildingButtons();
    }

    private void Update()
    {
        if (buildingDetail.isConstructing)
        {
            if (GameManager.inputManager.Press)
            {
                var pos = GameManager.inputManager.WorldPos;
                var index = vm.gridMap.PosToIndex(pos);
                var tile = vm.gridMap.GetTile(index.x, index.y);

                var constructObj = buildingDetail.ConstructBuilding(tile);
                if (constructObj != null)
                {
                    buildingDetail.isConstructing = false;
                    buildings.TryGetValue(um.currentBuildingData, out var obj);

                    ApplyBuildingEffection(constructObj.GetComponent<Building>());
                    
                    CheckBuildingButton(um.currentBuildingData, obj);
                    SortBuildingButtons();
                }
                else
                {
                    buildingDetail.isConstructing = false;
                }
                
            }
        }

        if (isReplacing)
        {
            if (GameManager.inputManager.Press)
            {
                var prevTile = um.currentNormalBuidling.standardTile;
                var isFlip = um.currentNormalBuidling.isFlip;
                var pos = GameManager.inputManager.WorldPos;
                var index = vm.gridMap.PosToIndex(pos);
                var tile = vm.gridMap.GetTile(index.x, index.y);

                SavePrevUnits();

                if (constructMode.construct.ForceRemovingBuilding(um.currentNormalBuidling, vm.gridMap))
                {
                    foreach(var data in buildingDatas)
                    {
                        if(data.StructureId == um.currentNormalBuidling.StructureId)
                        {
                            um.currentBuildingData = data;
                            break;
                        }
                    }
                    var b = buildingDetail.ConstructBuilding(tile);
                    if (b == null)
                    {
                        var obj = buildingDetail.ConstructBuilding(prevTile); 
                        um.currentNormalBuidling = obj.GetComponent<Building>();
                        if (um.currentNormalBuidling.StructureType == STRUCTURE_TYPE.PARAMETER_RECOVERY)
                        {
                            ReplaceFailParameterHandle();
                        }
                        Debug.Log("설치할 수 없는 위치입니다.");
                        if (isFlip)
                        {
                            obj.GetComponent<Building>().RotateBuilding(obj.GetComponent<Building>());
                        }
                        
                    }
                    else if (um.currentNormalBuidling.StructureType == STRUCTURE_TYPE.PARAMETER_RECOVERY)
                    {
                        ParameterHandle(); 
                        if (isFlip)
                        {
                            b.GetComponent<Building>().RotateBuilding(b.GetComponent<Building>());
                        }
                    }
                }
                else
                {
                    buildings.TryGetValue(um.currentBuildingData, out var obj);
                    CheckBuildingButton(um.currentBuildingData, obj);
                    SortBuildingButtons();
                }

                isReplacing = false;
                um.uiDevelop.ConstructButtonsOff();
            }
        }
    }

    private void ParameterHandle()
    {
        var building = um.currentNormalBuidling;
        var parameterBuilding = building.GetComponent<ParameterRecoveryBuilding>();
        var movingUnits = new List<UnitOnVillage>(parameterBuilding.movingUnits);
        foreach (var unit in movingUnits)
        {
            unit.UpdateDestination(building.gameObject);
        }

        var interactingUnits = parameterBuilding.interactingUnits;

        for(int i = interactingUnits.Count -1; i >= 0; --i)
        {
            interactingUnits[i].UpdateDestination(building.gameObject);
            interactingUnits[i].isRecoveryQuited = true;
        }
    }

    private void SavePrevUnits()
    {
        var building = um.currentNormalBuidling;

        var parameterBuilding = building?.GetComponent<ParameterRecoveryBuilding>();
        if (parameterBuilding == null)
            return;
        prevMovingUnits = parameterBuilding.movingUnits;
        prevInteractingUnits = parameterBuilding.interactingUnits;
    }

    private void ReplaceFailParameterHandle()
    {
        var building = um.currentNormalBuidling;
        for(int i = prevMovingUnits.Count -1; i >= 0; --i)
        {
            prevMovingUnits[i].UpdateDestination(building.gameObject);
        }

        for (int i = prevInteractingUnits.Count - 1; i >= 0; --i)
        {
            prevInteractingUnits[i].isRecoveryQuited = true;
            prevInteractingUnits[i].UpdateDestination(building.gameObject);
        }
    }

    private void MakeBuildingList()
    {
        if (buildings.Count != 0)
        {
            foreach (var building in buildings)
            {
                CheckBuildingButton(building.Key, building.Value);
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

            var frame = b.GetComponent<BuildingFrame>();
            frame.buildingImage.sprite = handle.Result.GetComponentInChildren<SpriteRenderer>().sprite;
            var button = frame.button;
            button.onClick.AddListener
            (() =>
            {
                //foreach(var building in vm.objectList.Values)
                //{
                //    if(building.GetComponent<Building>().StructureId == buildingData.StructureId)
                //    {
                //        um.currentNormalBuidling = building.GetComponent<Building>();
                //    }
                //}
                um.currentBuildingData = buildingData;
                var buildingDetailWindow = GameManager.uiManager.windows[WINDOW_NAME.BUILDING_DETAIL] as UIBuildingDetail;
                if (buildingDetailWindow != null)
                {
                    buildingDetailWindow.SetBuildingDetail();
                    buildingDetailWindow.Open();
                }
            });


            CheckBuildingButton(buildingData, b);
            buildings.Add(buildingData, b);
        }
    }

    private void CheckBuildingButton(BuildingData data, GameObject building)
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

        var button = building.GetComponent<BuildingFrame>().button;

        if (data.UpgradeId == 0) //portal
        {
            SetButtonColor(button, false);
            isActive = false;
            buttonActivationStatus[button] = isActive;
            return;
        }

        var upgradeData = DataTableManager.upgradeTable.GetData(data.UpgradeId)[grade -1];

        foreach (var tile in GameManager.villageManager.gridMap.tiles.Values)
        {
            if (!tile.tileInfo.ObjectLayer.IsEmpty
                && tile.tileInfo.ObjectLayer.LayerObject.GetComponentInChildren<Building>().StructureId == data.StructureId
                && !data.CanMultiBuild)
            {
                SetButtonColor(button, false);
                isActive = false;
                buttonActivationStatus[button] = isActive;
                return;
            }
        }

        if (data.UnlockTownLevel > GameManager.playerManager.level)
        {
            SetButtonColor(button, false);
            isActive = false;
            buttonActivationStatus[button] = isActive;
            return;
        }

        ////////////////TO-DO : 아이템 추가되면 주석 해제하기////////////////////////////////
        //for (int i = 0; i < upgradeData.ItemIds.Count; ++i)
        //{
        //    if (im.ownItemList.TryGetValue(upgradeData.ItemIds[i], out int itemNum))
        //    {
        //        if (im.ownItemList[upgradeData.ItemIds[i]] < upgradeData.ItemNums[i])
        //        {
        //            SetButtonColor(button, false);
        //            isActive = false;
        //            break;
        //        }
        //        else
        //        {
        //            SetButtonColor(button, true);
        //            isActive = true;
        //        }
        //    }
        //    else
        //    {
        //        SetButtonColor(button, false);
        //        isActive = false;
        //        break;
        //    }

        //}

        //모든 조건 검사 통과했을 때
        SetButtonColor(button, true);
        isActive = true;
        buttonActivationStatus[button] = isActive;
        return;

        if (isActive)
            SetButtonColor(button, true);

        buttonActivationStatus[button] = isActive;
    }

    private void SetButtonColor(Button button, bool satisfy)
    {
        if (satisfy)
        {
            button.interactable = true;
            button.targetGraphic.color = new Color(200f / 255f, 231f / 255f, 167f / 255f);
        }
        else
        {
            button.interactable = false;
            button.targetGraphic.color = new Color(255f / 255f, 128f / 255f, 128f / 255f);
        }
    }

    private void SortBuildingButtons()
    {
        List<Transform> children = new();

        foreach (Transform child in content)
        {
            children.Add(child);
        }

        children.Sort((a, b) =>
        {
            var dataA = GetBuildingDataFromObj(a.gameObject);
            var dataB = GetBuildingDataFromObj(b.gameObject);
            bool isActiveA = buttonActivationStatus[a.GetComponent<BuildingFrame>().button];
            bool isActiveB = buttonActivationStatus[b.GetComponent<BuildingFrame>().button];

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

    private BuildingData GetBuildingDataFromObj(GameObject obj)
    {
        foreach (var kvp in buildings)
        {
            if (kvp.Value == obj)
            {
                return kvp.Key;
            }
        }
        return null;
    }

    private void ApplyBuildingEffection(Building building)
    {
        var type = building.StructureType;
        switch(type)
        {
            case STRUCTURE_TYPE.STAT_UPGRADE:
                var statUp = building.GetComponent<StatUpgradeBuilding>();
                statUp.SetUpgradeStat(building);
                statUp.RiseStat();
                GameManager.unitManager.UnitUpgrade();
                break;
            case STRUCTURE_TYPE.PROGRESS:
                if(building.StructureId == (int)STRUCTURE_ID.STORAGE)
                {
                    var storage = building.GetComponent<StorageBuilding>();
                    var upgrade = building.GetComponent<BuildingUpgrade>();
                    storage.UpgradeGoldLimit((int)upgrade.ProgressVarReturn);
                }
                else if(building.StructureId == (int)STRUCTURE_ID.HOTEL)
                {
                    var hotel = building.GetComponent<HotelBuilding>();
                    var upgrade = building.GetComponent<BuildingUpgrade>();
                    hotel.UpgradeUnitLimit((int)upgrade.ProgressVarReturn);
                }
                break;

        }
    }

    private void ResetBuildingEffection(Building building)
    {
        var type = building.StructureType;
        switch (type)
        {
            case STRUCTURE_TYPE.STAT_UPGRADE:
                var statUp = um.currentNormalBuidling.GetComponent<StatUpgradeBuilding>();
                statUp.upgradeValue = 0;
                statUp.RiseStat();
                GameManager.unitManager.UnitUpgrade();
                break;
            case STRUCTURE_TYPE.PROGRESS:
                if (building.StructureId == (int)STRUCTURE_ID.STORAGE)
                {
                    var storage = building.GetComponent<StorageBuilding>();
                    var upgrade = building.GetComponent<BuildingUpgrade>();
                    storage.UpgradeGoldLimit(storage.DefaultGoldLimit);
                }
                else if(building.StructureId == (int)STRUCTURE_ID.HOTEL)
                {
                    var hotel = building.GetComponent<HotelBuilding>();
                    var limit = GameManager.unitManager.unitLimitCount;
                    hotel.UpgradeUnitLimit(limit);
                }
                break;
        }
    }
}
