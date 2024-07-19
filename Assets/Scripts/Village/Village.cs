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
    public BuildingUpgrade upgrade;

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
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 100f);

            if (hit.collider != null)
            {
                var building = hit.transform.gameObject.GetComponent<Building>();

                if (building != null
                    /*&& this == building*/)
                {
                    Debug.Log(building.StructureName);
                    upgrade = building.gameObject.GetComponent<BuildingUpgrade>();
                }
                else
                {
                    upgrade = null;
                }
            }
        }

        timer += Time.deltaTime;
        if (timer >= 5f)
        {
            timer = 0f;
            Debug.Log($"str : {GameManager.playerManager.upgradeSTR}, mag : {GameManager.playerManager.upgradeMAG}, agi : {GameManager.playerManager.upgradeAGI}");
        }
    }

    public void Upgrade()
    {
        Debug.Log("업그레이드");
        upgrade?.Upgrade();
    }

    public void Cancel()
    {
        this.gameObject.SetActive(false);
    }
}
