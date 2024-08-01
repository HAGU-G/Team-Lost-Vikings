using System.Collections.Generic;
using UnityEngine;

public class Village : MonoBehaviour
{
    public VillageManager villageManager;
    public UnitOnVillage unitPrefab;
    public List<UnitOnVillage> units;
    public List<UnitStats> unitStats;
    public StatsData unitStatsData; 
    public BuildingUpgrade upgrade;

    public PlayerManager pm;

    private float timer = 0f;

    private void Awake()
    {
        units = new List<UnitOnVillage>();
        pm = GameManager.playerManager;

        GameManager.Subscribe(EVENT_TYPE.INIT, OnGameInit);
    }

    private void OnGameInit()
    {
        
    }

    //public void UnitSpawn()
    //{
    //    foreach (var unitSelected in GameManager.unitManager.Units)
    //    {
    //        var unit = unitSelected.Value;
    //        if (unit.Location != LOCATION.NONE)
    //            continue;

    //        var unitObj = GameObject.Instantiate(unitPrefab, villageManager.gridMap.IndexToPos(new Vector2Int(35, 31))
    //            , Quaternion.identity, villageManager.gridMap.transform);

    //        unitObj.Init();
    //        unitObj.ResetUnit(unitSelected.Value);
    //        units.Add(unitObj);
    //    }
    //}

    public void UnitSpawn(int instanceID, STRUCTURE_TYPE spawnBuilding)
    {
        Vector3 spawnPos = new();

        bool isFind = false ;
        foreach (var constructed in GameManager.villageManager.constructedBuildings)
        {
            var building = constructed.GetComponent<Building>();
            if (building.StructureType == spawnBuilding)
            {
                spawnPos = building.entranceTile.gameObject.transform.position;
                isFind = true;
                break;
            }
        }

        if(!isFind)
        {
            foreach (var constructed in GameManager.villageManager.constructedBuildings)
            {
                var building = constructed.GetComponent<Building>();
                if (building.StructureType == STRUCTURE_TYPE.STANDARD)
                {
                    spawnPos = building.entranceTile.gameObject.transform.position;
                    break;
                }
            }
        }



        var unitObj = Instantiate(unitPrefab, spawnPos, Quaternion.identity, villageManager.gridMap.transform);
        unitObj.Init();
        unitObj.ResetUnit(GameManager.unitManager.GetUnit(instanceID));
        units.Add(unitObj);
        unitObj.stats.OnArrived();
    }


    //private void OnGUI()
    //{
    //    if (GUI.Button(new Rect(0f, 0f, 100f, 70f), "ReduceHp"))
    //    {
    //        ReduceHp();
    //    }

    //    if (GUI.Button(new Rect(0f, 70f, 100f, 70f), "ReduceStamina"))
    //    {
    //        ReduceStamina();
    //    }

    //    if (GUI.Button(new Rect(0f, 140f, 100f, 70f), "ReduceStress"))
    //    {
    //        ReduceStress();
    //    }

    //    if (GUI.Button(new Rect(0f, 210f, 100f, 70f), "GoHunZone"))
    //    {

    //    }

    //    if (GUI.Button(new Rect(0f, 420f, 100f, 70f), "Unit Spawn"))
    //    {
    //        UnitSpawn();
    //    }
    //}
    //-------Test용 메소드-----------------------------------------------

    public void GoHunt(int instanceID)
    {
        GoHunt(units[
            units.FindIndex(
            (x) =>
            {
                return x.stats.InstanceID == instanceID;
            })]);
    }

    public void GoHunt(UnitOnVillage unit)
    {
        if (!units.Contains(unit)
            || !GameManager.huntZoneManager.HuntZones.ContainsKey(unit.stats.HuntZoneNum))
            return;

        units.Remove(unit);
        unit.stats.SetLocation(LOCATION.NONE, LOCATION.HUNTZONE);
        GameManager.unitManager.SpawnOnNextLocation(unit.stats);
        Destroy(unit.gameObject);
    }

    public void ReduceHp()
    {
        foreach (var unit in units)
            unit.stats.HP.Current -= 30;
    }
    public void ReduceStamina()
    {
        foreach (var unit in units)
            unit.stats.Stamina.Current -= 30;
    }

    public void ReduceStress()
    {
        foreach (var unit in units)
            unit.stats.Stress.Current -= 30;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 5f)
        {
            timer = 0f;
            //Debug.Log($"str : {pm.unitStr.defaultValue}, mag : {pm.unitMag.defaultValue}, agi : {pm.unitAgi.defaultValue}");
        }
    }

    public void Upgrade()
    {
        Debug.Log("업그레이드");
        upgrade?.Upgrade();
    }

    public void Cancel()
    {
        upgrade = null;
    }
}
