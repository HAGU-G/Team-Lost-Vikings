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

    public List<Tile> placedTiles = new List<Tile>();
    public Tile entranceTile;
    private bool isFlip = false;

    public IInteractableWithPlayer interactWithPlayer { get; private set; }
    public IInteractableWithUnit interactWithUnit { get; private set; }

    private void Awake()
    {
        
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
                if (building != null)
                {
                    //RotateBuilding();
                }
            }
        }
    }

    private void OnGUI()
    {
        
    }

    public void RotateBuilding()
    {
        var trans = gameObject.transform.rotation;
        if (!isFlip)
        {
            trans.y += 180f;
            isFlip = true;
        }
        else
        {
            trans.y -= 180f;
            isFlip = false;
        }
            
        gameObject.transform.rotation = trans;

        var tileId = entranceTile.tileInfo.id;
        tileId.x += 1;
        tileId.y -= 1;
        entranceTile.ResetTileInfo();
        entranceTile.tileInfo.id = tileId;
        Debug.Log(entranceTile.tileInfo.id);
    }
}
