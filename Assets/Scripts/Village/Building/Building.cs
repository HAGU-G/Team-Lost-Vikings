using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;


public class Building : MonoBehaviour, IPointerClickHandler
{
    //데이터 테이블에서 받아오기 전 임시로 입력
    [field: SerializeField]
    public string StructureName { get; set; }
    [field: SerializeField]
    public int StructureId { get; set; }
    [field: SerializeField]
    public int Width { get; set; }
    [field: SerializeField]
    public int Length { get; set; }
    [field: SerializeField]
    public STRUCTURE_TYPE StructureType { get; set; }
    [field: SerializeField]
    public int UnlockTownLevel { get; set; }
    [field: SerializeField]
    public bool CanMultiBuild { get; set; }
    [field: SerializeField]
    public bool CanReverse { get; set; }
    [field: SerializeField]
    public bool CanReplace { get; set; }
    [field: SerializeField]
    public bool CanDestroy { get; set; }
    [field: SerializeField]
    public int UpgradeId { get; set; }
    [field: SerializeField]
    public string StructureAssetFileName { get; set; }
    [field: SerializeField]
    public string StructureDesc { get; set; }

    public Cell standardTile; //설치, 회전 시 기준이 되는 타일
    public List<Cell> placedTiles = new (); //건물 프리팹이 차지하는 모든 타일
    public List<Cell> marginTiles = new(); //여백에 해당하는 타일
    public List<Cell> realOccupiedTiles = new(); //여백을 제외한 실제 건물이 설치되는 타일

    public List<Cell> entranceTiles = new();
    public bool isFlip = false;
    //private static bool isRotating = false;
    public GridMap gridMap;


    public IInteractableWithPlayer interactWithPlayer { get; private set; }
    public IInteractableWithUnit interactWithUnit { get; private set; }

    private SortingGroup sortingGroup = null;


    private void Awake()
    {
        //gridMap = GameObject.FindWithTag("GridMap").GetComponent<GridMap>();
    }

    private void Start()
    {
        interactWithPlayer = gameObject.GetComponent<IInteractableWithPlayer>();
        interactWithUnit = gameObject.GetComponent<IInteractableWithUnit>();

        sortingGroup = gameObject.GetComponent<SortingGroup>();
        if (sortingGroup == null)
            sortingGroup = gameObject.AddComponent<SortingGroup>();
        sortingGroup.sortAtRoot = true;
        sortingGroup.sortingOrder = Mathf.FloorToInt(-transform.position.y);
    }

    public void Interact()
    {
        interactWithPlayer?.InteractWithPlayer();
        interactWithUnit?.InteractWithUnit(new UnitOnVillage());
        //TO-DO : 수정하기
    }

    public void TouchBuilding()
    {
        if(!GameManager.villageManager.constructMode.isConstructMode)
        {
            GameManager.uiManager.currentNormalBuidling = this;
            GameManager.uiManager.windows[WINDOW_NAME.BUILDING_POPUP].Open();
        }
        else if(GameManager.villageManager.constructMode.isConstructMode)
        {
            GameManager.uiManager.currentNormalBuidling = this;
            GameManager.uiManager.uiDevelop.TouchBuildingInConstructMode();
        }
       
    }

    public void RotateBuilding(Building building)
    {
        var localScale = building.transform.localScale;
        Vector3 textScale;

        //var transedId = building.entranceTile.tileInfo.id;
        //if (building.entranceTile.tileInfo.RoadLayer.LayerObject != null)
        //{
        //    building.entranceTile.tileInfo.RoadLayer.LayerObject.GetComponent<
        //        Renderer>().material.color = default;
        //}

        gridMap = building.gridMap;
        if (!building.isFlip)
        {
            //transedId.x += 1;
            //transedId.y -= 1;
            //if (!gridMap.usingTileList.Contains(gridMap.tiles[transedId])
            //    || gridMap.tiles[transedId].tileInfo.TileType == TileType.OBJECT)
            //    return;

            textScale = building.gameObject.GetComponentInChildren<TextMeshPro>().gameObject.transform.localScale;
            textScale.x *= -1;
            localScale.x *= -1;
            building.isFlip = true;
        }
        else
        {
            //transedId.x -= 1;
            //transedId.y += 1;
            //if (!gridMap.usingTileList.Contains(gridMap.tiles[transedId])
            //    || gridMap.tiles[transedId].tileInfo.TileType == TileType.OBJECT)
            //    return;

            textScale = building.gameObject.GetComponentInChildren<TextMeshPro>().gameObject.transform.localScale;
            textScale.x *= -1;
            localScale.x *= -1;
            building.isFlip = false;
        }
        building.transform.localScale = localScale;
        building.gameObject.GetComponentInChildren<TextMeshPro>().gameObject.transform.localScale = textScale;

        //building.entranceTile.ResetTileInfo();
        //building.entranceTile = gridMap.tiles[transedId];
        //building.entranceTile.TileColorChange();
        //if (building.entranceTile.tileInfo.RoadLayer.LayerObject != null)
        //{
        //    //building.entranceTile.tileInfo.RoadLayer.LayerObject.GetComponent<SpriteRenderer>().material.color = Color.magenta;
        //}
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //if (GameManager.uiManager.isWindowOn)
        //    return;

        var building = GetComponent<Building>();
        var parameter = GetComponent<ParameterRecoveryBuilding>();
        if (parameter != null)
        {
            parameter.TouchParameterBuilding();
        }
        else
        {
            TouchBuilding();
        }

        GameManager.villageManager.village.upgrade = gameObject.GetComponent<BuildingUpgrade>();
    }
}
