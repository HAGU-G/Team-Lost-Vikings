using CsvHelper;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.GraphicsBuffer;

[JsonObject(MemberSerialization.OptIn)]
public class QuestManager
{
    /// <summary>
    /// 업적 ID, 업적 카운트
    /// </summary>
    [JsonProperty] public Dictionary<int, int> Achievements = new();

    /// <summary>
    /// 퀘스트ID, 클리어 여부
    /// </summary>
    [JsonProperty] public Dictionary<int, bool> GuideQuests = new();

    public void LoadAchievements()
    {
        foreach (var achieve in DataTableManager.achievementTable.GetDatas())
        {
            if (!Achievements.ContainsKey(achieve.AchieveId))
                Achievements.Add(achieve.AchieveId, 0);
        }
    }

    public void LoadQuests()
    {
        foreach (var quest in DataTableManager.questTable.GetDatas())
        {
            if (!GuideQuests.ContainsKey(quest.Id))
                GuideQuests.Add(quest.Id, false);
        }
    }

    public void SetAchievementCountByTargetID(int targetID, ACHIEVEMENT_TYPE type, int count)
    {
        var achieveID = 0;
        foreach (var achieve in DataTableManager.achievementTable.GetDatas())
        {
            if (achieve.TargetId == targetID
                && achieve.AchieveType == type)
                achieveID = achieve.AchieveId;
        }

        if (achieveID == 0)
        {
            Debug.Log($"{targetID}를 사용하는 {type}의 업적이 없습니다.");
            return;
        }

        if (type == ACHIEVEMENT_TYPE.BUILDING_BUILD
            && Achievements[achieveID] >= 1)
        {
            Debug.Log($"{targetID}를 사용하는 {type}의 업적을 이미 달성했습니다.");
            return;
        }
        SetAchievementCount(achieveID, count);
    }

    public void SetAchievementCount(int id, int count)
    {

        if (!Achievements.ContainsKey(id))
        {
            Debug.LogError($"업적 {id}이(가) 없습니다.");
            return;
        }

        if(DataTableManager.achievementTable.GetData(id).AchieveType == ACHIEVEMENT_TYPE.BUILDING_BUILD
            && Achievements[id] >= 1)
        {
            Debug.Log($"업적 {id}을(를) 이미 달성했습니다.");
            return;
        }

        Achievements[id] += count;
    }

    public void QuestClear(int id, bool isClear = true)
    {
        if (!GuideQuests.ContainsKey(id))
        {
            Debug.LogError($"퀘스트 {id}이(가) 없습니다.");
            return;
        }

        GuideQuests[id] = isClear;
    }
}
