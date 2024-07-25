using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Building : MonoBehaviour
{
    //데이터 테이블에서 받아오기 전 임시로 입력
    [field:SerializeField] 
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

    public List<Cell> placedTiles = new List<Cell>();
    public Cell entranceTile;
    private bool isFlip = false;
    //private static bool isRotating = false;
    public GridMap gridMap;
    

    public IInteractableWithPlayer interactWithPlayer { get; private set; }
    public IInteractableWithUnit interactWithUnit { get; private set; }

    private void Awake()
    {
        //gridMap = GameObject.FindWithTag("GridMap").GetComponent<GridMap>();
    }

    private void Start()
    {
        interactWithPlayer = gameObject.GetComponent<IInteractableWithPlayer>();
        interactWithUnit = gameObject.GetComponent<IInteractableWithUnit>();
    }

    public void Interact()
    {
        interactWithPlayer?.InteractWithPlayer();
        interactWithUnit?.InteractWithUnit(new UnitOnVillage());
        //TO-DO : 수정하기
    }

    public void TouchBuilding()
    {
        GameManager.uiManager.currentNormalBuidling = this;
        GameManager.uiManager.windows[WINDOW_NAME.BUILDING_POPUP].Open();
    }

    private void Update()
    {
        
    }


    //private void OnGUI()
    //{
    //    if(GUI.Button(new Rect(0f, 280f, 100f, 70f), "Rotate"))
    //    {
    //        isRotating = true;
    //    }
    //}

    public void RotateBuilding(Building building)
    {
        var localScale = building.transform.localScale;
        var transedId = building.entranceTile.tileInfo.id;
        Vector3 textScale;
        if (building.entranceTile.tileInfo.RoadLayer.LayerObject != null)
        {
            building.entranceTile.tileInfo.RoadLayer.LayerObject.GetComponent<
                Renderer>().material.color = default;
        }
        gridMap = building.gridMap;
        if (!building.isFlip)
        {
            transedId.x += 1;
            transedId.y -= 1;
            if (!gridMap.usingTileList.Contains(gridMap.tiles[transedId])
                || gridMap.tiles[transedId].tileInfo.TileType == TileType.OBJECT)
                return;

            textScale = building.gameObject.GetComponentInChildren<TextMeshPro>().gameObject.transform.localScale;
            textScale.x *= -1;
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
            textScale = building.gameObject.GetComponentInChildren<TextMeshPro>().gameObject.transform.localScale;
            textScale.x *= -1;
            localScale.x *= -1;
            building.isFlip = false;
        }
        building.transform.localScale = localScale;
        building.gameObject.GetComponentInChildren<TextMeshPro>().gameObject.transform.localScale = textScale;
        building.entranceTile.ResetTileInfo();
        building.entranceTile = gridMap.tiles[transedId];
        building.entranceTile.TileColorChange(); 
        if (building.entranceTile.tileInfo.RoadLayer.LayerObject != null)
        {
            //building.entranceTile.tileInfo.RoadLayer.LayerObject.GetComponent<SpriteRenderer>().material.color = Color.magenta;
        }
    }
}
