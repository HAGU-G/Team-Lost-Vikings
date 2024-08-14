using CsvHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

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
    [JsonProperty] public SortedDictionary<int, bool> GuideQuests = new();
    [JsonProperty] private int currentQuestID = -1;
    public QuestData CurrentQuest
    {
        get
        {
            if (DataTableManager.questTable.ContainsKey(currentQuestID))
                return DataTableManager.questTable.GetData(currentQuestID);
            else
                return null;
        }
    }
    public bool IsAllClear
    {
        get
        {
            bool result = true;
            foreach (var isClear in GuideQuests.Values)
            {
                result &= isClear;
            }
            return result;
        }
    }

    public event Action OnQuestAccepted = null;
    public event Action OnQuestSatisfied = null;
    public event Action OnQuestCleared = null;
    public event Action OnAchievementUpdated = null;


    public void LoadAchievements()
    {
        foreach (var achieve in DataTableManager.achievementTable.GetDatas())
        {
            if (!Achievements.ContainsKey(achieve.AchieveId))
            {
                var count = achieve.AchieveType switch
                {
                    ACHIEVEMENT_TYPE.BUILDING_UPGRADE => 1,
                    _ => 0
                };
                Achievements.Add(achieve.AchieveId, count);
            }
        }
    }

    public void LoadQuests()
    {
        foreach (var quest in DataTableManager.questTable.GetDatas())
        {
            if (!GuideQuests.ContainsKey(quest.Id) && quest.QuestType == QUEST_TYPE.GUIDE)
                GuideQuests.Add(quest.Id, false);
        }

        if (CurrentQuest == null && !IsAllClear)
        {
            foreach (var quest in GuideQuests)
            {
                if (!quest.Value)
                {
                    QuestAccept(quest.Key);
                    break;
                }
            }
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

        if (DataTableManager.achievementTable.GetData(id).AchieveType == ACHIEVEMENT_TYPE.BUILDING_BUILD
            && Achievements[id] >= 1)
        {
            Debug.Log($"업적 {id}을(를) 이미 달성했습니다.");
            return;
        }

        Achievements[id] += count;
        OnAchievementUpdated?.Invoke();
        if (CurrentQuest != null)
            CheckQuestSatisfy(CurrentQuest.Id);
    }

    private void CheckQuestSatisfy(int id)
    {
        var quest = DataTableManager.questTable.GetData(id);
        if (quest.IsSatisfied)
        {
            OnQuestSatisfied?.Invoke();

            var dialogID = DataTableManager.questTable.GetData(id).DialogId2;
            if (dialogID != 0)
                GameManager.dialogManager.Book(dialogID);

            if (quest.CanAutoClear == 1)
                QuestClear(quest.Id);
        }
    }

    public void QuestClear(int id, bool isClear = true)
    {
        if (!GuideQuests.ContainsKey(id))
        {
            Debug.LogError($"퀘스트 {id}이(가) 없습니다.");
            return;
        }

        GuideQuests[id] = isClear;
        OnQuestCleared?.Invoke();
        var questData = DataTableManager.questTable.GetData(id);
        var dialogID = questData.DialogId3;
        if (dialogID == 0)
        {
            DataTableManager.questTable.GetData(id).GetReward();
        }
        else
        {
            GameManager.dialogManager.Book(dialogID);
            GameManager.dialogManager.BookReward(dialogID, questData);
        }

        var nextQuestID = id + 1;
        if (GuideQuests.ContainsKey(nextQuestID))
            QuestAccept(nextQuestID);
        else
            currentQuestID = -1;
    }

    private void QuestAccept(int id)
    {
        currentQuestID = id;
        OnQuestAccepted?.Invoke();
        var dialogID = DataTableManager.questTable.GetData(id).DialogId1;
        if (dialogID != 0)
            GameManager.dialogManager.Book(dialogID);
        CheckQuestSatisfy(id);
    }

}
