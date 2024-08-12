using Newtonsoft.Json;
using System.Collections.Generic;

[JsonObject(MemberSerialization.OptIn)]
public class QuestManager
{
    /// <summary>
    /// 업적 ID, 업적 카운트
    /// </summary>
    [JsonProperty] public Dictionary<int, int> Achivements = new();
   
    /// <summary>
    /// 퀘스트ID, 클리어 여부
    /// </summary>
    [JsonProperty] public Dictionary<int, bool> Quests = new();
}
