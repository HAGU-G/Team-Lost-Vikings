using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class PlayerManager
{
    [JsonProperty] public int level;

    [JsonProperty] public StatInt unitStr = new();
    [JsonProperty] public StatInt unitMag = new();
    [JsonProperty] public StatInt unitAgi = new();
}