using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village : MonoBehaviour
{
    public VillageManager villageManager;
    public UnitOnVillage unitPrefab;
    public List<UnitOnVillage> units;

    private float timer = 0f;

    private void Awake()
    {
       units = new List<UnitOnVillage>();
    }

    private void Start()
    {
        var unit = Instantiate(unitPrefab, villageManager.gridMap.IndexToPos(new Vector2Int(31, 31)), Quaternion.identity, villageManager.gridMap.transform);
        units.Add(unit);
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
