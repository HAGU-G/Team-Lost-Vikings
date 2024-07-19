using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Village : MonoBehaviour
{
    public VillageManager villageManager;
    public UnitOnVillage unitPrefab;
    public List<UnitOnVillage> units;
    public List<UnitStats> unitStats;
    public UnitStatsData unitStatsData;

    private float timer = 0f;

    private void Awake()
    {
        units = new List<UnitOnVillage>();
    }

    private void Start()
    {
        //var unit = Instantiate(unitPrefab, villageManager.gridMap.IndexToPos(new Vector2Int(31, 31))
        //    , Quaternion.identity, villageManager.gridMap.transform);
        //units.Add(unit);


    }

    public void UnitSpawn()
    {
        foreach (var unitSelected in GameManager.unitManager.Units)
        {
            var unit = unitSelected.Value;
            if (unit.Location != LOCATION.NONE)
                continue;

            var unitObj = GameObject.Instantiate(unitPrefab, villageManager.gridMap.IndexToPos(new Vector2Int(35, 31))
                , Quaternion.identity, villageManager.gridMap.transform);

            unitObj.Init();
            unitObj.ResetUnit(unitSelected.Value);
            units.Add(unitObj);
        }
    }

    public void UnitSpawn(int instanceID)
    {
        var unitObj = GameObject.Instantiate(unitPrefab, villageManager.gridMap.IndexToPos(new Vector2Int(35, 31))
                , Quaternion.identity, villageManager.gridMap.transform);
        unitObj.Init();
        unitObj.ResetUnit(GameManager.unitManager.GetUnit(instanceID));
        units.Add(unitObj);
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
            || !GameManager.huntZoneManager.HuntZones.ContainsKey(unit.stats.HuntZoneID))
            return;

        units.Remove(unit);
        unit.stats.SetLocation(LOCATION.NONE, LOCATION.HUNTZONE);
        GameManager.unitManager.SpawnOnLocation(unit.stats);
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
        //timer += Time.deltaTime;
        //if (timer >= 10f)
        //{
        //    foreach (var unit in units)
        //    {
        //        timer = 0f;
        //        Debug.Log($"str : {unit.stats.Str.Current} / mag : {unit.stats.Mag.Current} / agi : {unit.stats.Agi.Current}");
        //    }
        //}
    }
}
