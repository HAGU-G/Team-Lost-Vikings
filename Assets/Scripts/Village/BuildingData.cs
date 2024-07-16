using UnityEngine;

public class BuildingData : MonoBehaviour
{
    VillageManager villageManager;
    GameObject buildingPrefab;

    private void Awake()
    {
        GameStarter.Instance.SetActiveOnComplete(gameObject);
    }

    //private void Start()
    //{
    //    //데이터 테이블에서 값을 읽어와서 할당해주기
    //    for (int i = 0; i < table.Length; ++i)
    //    {
    //        var buildingComponenet = buildingPrefab.AddComponent<Building>();
    //        buildingComponenet.StructureName = table[i].Name;
    //        buildingComponenet.StructureId = table[i].Id;
    //        buildingComponenet.Width = table[i].Width;
    //        buildingComponenet.Length = table[i].Length;
    //        buildingComponenet.StructureType = table[i].StructureType;
    //        buildingComponenet.UnlockTownLevel = table[i].UnlockTownLevel;
    //        buildingComponenet.CanReverse = table[i].CanReverse;
    //        buildingComponenet.CanReplace = table[i].CanReplace;
    //        buildingComponenet.CanDestroy = table[i].CanDestroy;
    //        buildingComponenet.UpgradeId = table[i].UpgradeId;
    //        buildingComponenet.StructureAssetFileName = table[i].AssetFileName;

    //        var sprite = buildingPrefab.GetComponent<SpriteRenderer>();
    //        sprite.sprite =  //스프라이트 할당

    //        switch (building.StructureType)
    //        {
    //            case STRUCTURE_TYPE.PARAMETER_RECOVERY:
    //                buildingPrefab.AddComponent<ParameterRecoveryBuilding>();
    //                break;
    //            case STRUCTURE_TYPE.STAT_UPGRADE:
    //                buildingPrefab.AddComponent<StatUpgradeBuilding>();
    //                break;
    //            case STRUCTURE_TYPE.ITEM_SELL:
    //                buildingPrefab.AddComponent<ItemSellBuilding>();
    //                break;
    //            case STRUCTURE_TYPE.ITEM_PRODUCE:
    //                buildingPrefab.AddComponent<ItemProduceBuilding>();
    //                break;
    //        }
    //    }
    //    var b = Instantiate(buildingPrefab);
    //    villageManager.installableBuilding.Add(b);
    //}
}
