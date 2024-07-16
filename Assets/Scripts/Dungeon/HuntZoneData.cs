using System;
using UnityEngine;

[Serializable]
public class HuntZoneData
{
    [field: SerializeField] public string HuntZoneName { get; set; }
    [field: SerializeField] public int HuntZoneId { get; set; }
    [field: SerializeField] public int HuntZoneNum { get; set; }
    [field: SerializeField] public int HuntZoneStage { get; set; }
    [field: SerializeField] public int UnitCapacity { get; set; }
    [field: SerializeField] public int NormalMonsterId {  get; set; }
    [field: SerializeField] public int MaxMonNum { get; set; }
    [field: SerializeField] public float MonRegen { get; set; }
    [field: SerializeField] public int BossMonsterId { get; set; }
    [field: SerializeField] public float BossKillTimer { get; set; }
    [field: SerializeField] public float BossRetryTimer { get; set; }
}