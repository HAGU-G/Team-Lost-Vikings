using System.Collections.Generic;
using UnityEngine;

public class Village : MonoBehaviour
{
    public VillageManager villageManager;
    public UnitOnVillage unitPrefab;
    public List<UnitOnVillage> units;
    public List<UnitStats> unitStats;

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

            var unitOnVillage = GameManager.villageManager.GetUnit(unit);
            unitOnVillage.transform.position = transform.position;
            Debug.Log(unit.InstanceID + "소환됨", unitOnVillage.gameObject);
            break;
        }
    }

    private void OnGUI()
    {
        if(GUI.Button(new Rect(440f, 420f, 100f, 70f), "Unit Spawn"))
         {
            UnitSpawn();
         }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 10f)
        {
            foreach (var unit in units)
            {
                timer = 0f;
                Debug.Log($"str : {unit.stats.Str.Current} / mag : {unit.stats.Mag.Current} / agi : {unit.stats.Agi.Current}");
            }
        }
    }
}
