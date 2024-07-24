using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class HuntZoneInfo
{
    [field: SerializeField] public int HuntZoneNum { get; set; }
    [field: SerializeField] public int Stage { get; set; } = 1;
    [field: SerializeField] public float RetryTimer { get; set; }
    public bool CanSpawnBoss { get; set; } = true;
}