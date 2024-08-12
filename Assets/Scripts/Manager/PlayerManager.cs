using Newtonsoft.Json;
using System.Collections.Generic;

[JsonObject(MemberSerialization.OptIn)]
public class PlayerManager
{
    [JsonProperty] public int level;

    [JsonProperty] public StatInt unitStr = new();
    [JsonProperty] public StatInt unitMag = new();
    [JsonProperty] public StatInt unitAgi = new();
    public StatFloat unitCritChance = new();
    public StatFloat unitCritWeight = new();
    public StatInt unitHp = new();
    public StatInt unitStamina = new();
    public StatInt unitMental = new();

    public Dictionary<int, int> buildingUpgradeGrades = new(); //structureId, upgradeGrade
}