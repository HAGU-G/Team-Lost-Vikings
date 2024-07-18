using System;
using System.Collections.Generic;
using UnityEngine;


public class Building : MonoBehaviour
{
    [field:SerializeField] //데이터 테이블에서 받아오기 전 임시로 입력
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

    public List<Cell> placedTiles = new List<Cell>();
    public Cell entranceTile;
    private bool isFlip = false;
    private static bool isRotating = false;
    public GridMap gridMap;

    public IInteractableWithPlayer interactWithPlayer { get; private set; }
    public IInteractableWithUnit interactWithUnit { get; private set; }

    private void Awake()
    {
        //gridMap = GameObject.FindWithTag("GridMap").GetComponent<GridMap>();
    }

    private void Start()
    {
        
        switch (StructureType)
        {
            case STRUCTURE_TYPE.PARAMETER_RECOVERY:
                interactWithUnit = gameObject.GetComponent<ParameterRecoveryBuilding>();
                break;
            case STRUCTURE_TYPE.STAT_UPGRADE:
                interactWithPlayer = gameObject.GetComponent<StatUpgradeBuilding>();
                break;
            case STRUCTURE_TYPE.ITEM_PRODUCE:
                interactWithPlayer = gameObject.GetComponent<ItemProduceBuilding>();
                break;
            case STRUCTURE_TYPE.ITEM_SELL:
                interactWithPlayer = gameObject.GetComponent<ItemSellBuilding>();
                break;
        }
    }

    public void Interact()
    {
        interactWithPlayer?.InteractWithPlayer();
        interactWithUnit?.InteractWithUnit(new UnitOnVillage());
        //TO-DO : 수정하기
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 100f);

            if (hit.collider != null)
            {
                var building = hit.transform.gameObject.GetComponent<Building>();
                if (building != null && isRotating)
                {
                    RotateBuilding(building);
                    isRotating = false;
                }
            }
        }
    }

    private void OnGUI()
    {
        if(GUI.Button(new Rect(0f, 280f, 100f, 70f), "Rotate"))
        {
            isRotating = true;
        }
    }

    public void RotateBuilding(Building building)
    {
        var localScale = building.transform.localScale;
        var transedId = building.entranceTile.tileInfo.id;
        gridMap = building.gridMap;
        if (!building.isFlip)
        {
            transedId.x += 1;
            transedId.y -= 1;
            Debug.Log(gridMap);
            if (!gridMap.usingTileList.Contains(gridMap.tiles[transedId])
                || gridMap.tiles[transedId].tileInfo.TileType == TileType.OBJECT)
                return;

            localScale.x *= -1;
            building.isFlip = true;
        }
        else
        {
            transedId.x -= 1;
            transedId.y += 1;
            Debug.Log(gridMap);
            if (!gridMap.usingTileList.Contains(gridMap.tiles[transedId])
                || gridMap.tiles[transedId].tileInfo.TileType == TileType.OBJECT)
                return;

            localScale.x *= -1;
            building.isFlip = false;
        }
        building.transform.localScale = localScale;
        building.entranceTile.ResetTileInfo();
        building.entranceTile = gridMap.tiles[transedId];
        building.entranceTile.TileColorChange();
    }
}
