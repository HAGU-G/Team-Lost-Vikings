﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village : MonoBehaviour
{
    public VillageManager villageManager;
    public UnitOnVillage unitPrefab;

    private void Awake()
    {
        GameStarter.Instance.SetActiveOnComplete(gameObject);
    }

    private void Start()
    {
        var unit = Instantiate(unitPrefab, villageManager.gridMap.IndexToPos(new Vector2Int(0, 0)), Quaternion.identity, villageManager.gridMap.transform);
        
    }
}
