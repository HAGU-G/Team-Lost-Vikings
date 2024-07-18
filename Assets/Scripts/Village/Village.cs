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

    private void UnitSpawn()
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

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0f, 0f, 100f, 70f), "ReduceHp"))
        {
            ReduceHp();
        }

        if (GUI.Button(new Rect(0f, 70f, 100f, 70f), "ReduceStamina"))
        {
            ReduceStamina();
        }

        if (GUI.Button(new Rect(0f, 140f, 100f, 70f), "ReduceStress"))
        {
            ReduceStress();
        }

        if (GUI.Button(new Rect(0f, 210f, 100f, 70f), "GoHunZone"))
        {
            var destroy = new List<UnitOnVillage>();
            foreach (var unit in units)
            {
                if (unit.currentState != UnitOnVillage.STATE.IDLE)
                    continue;

                destroy.Add(unit);
                unit.stats.SetLocation(LOCATION.NONE);
            }

            foreach (var unit in destroy)
            {
                units.Remove(unit);
                Destroy(unit.gameObject);
            }
        }

        if (GUI.Button(new Rect(0f, 420f, 100f, 70f), "Unit Spawn"))
        {
            UnitSpawn();
        }
    }
    //-------Test용 메소드-----------------------------------------------


    private void ReduceHp()
    {
        foreach (var unit in units)
            unit.stats.HP.Current -= 30;
    }
    public void ReduceStamina()
    {
        foreach (var unit in units)
            unit.stats.Stamina.Current -= 30;
    }

    private void ReduceStress()
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
