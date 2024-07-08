using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum STRUCTURE_TYPE
{
    HOSPITAL = -1,
    STAT_UPGRADE,
    PARAMETER_RECOVERY,
    ITEM_PRODUCE,
    ITEM_SELL,
    REVIVE,
}

public class Building : MonoBehaviour
{
    [field:SerializeField] //데이터 테이블에서 받아오기 전 임시로 입력
    public string StructureName { get; set; }
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

    public List<Tile> placedTiles;

    private IInteractableWithPlayer interactWithPlayer;
    private IInteractableWithUnit interactWithUnit;

    private void Awake()
    {
        placedTiles = new List<Tile>();
        switch (StructureType)
        {
            case STRUCTURE_TYPE.PARAMETER_RECOVERY:
                interactWithPlayer = null;
                interactWithUnit = null;
                interactWithUnit = gameObject.GetComponent<ParameterRecoveryBuilding>();
                break;

        }
    }

    public void Interact()
    {
        interactWithPlayer?.InteractWithPlayer();
        interactWithUnit?.InteractWithUnit();
    }
}
