using CsvHelper;
using System.Collections.Generic;
using UnityEditor;

public enum QUEST_TYPE
{
    NONE,
    GUIDE,
    TUTORIAL
}

public class QuestData : ITableAvaialable<int>, ITableExtraLoadable
{
    public string Name { get; set; }
    public int Id { get; set; }
    public QUEST_TYPE QuestType { get; set; }
    public int CanAutoClear { get; set; }
    public string QuestDesc { get; set; }
    public List<int> AchievementIDs { get; set; } = new();
    public List<int> RequireNums { get; set; } = new();
    public int DialogId1 { get; set; }
    public int DialogId2 { get; set; }
    public int DialogId3 { get; set; }
    public int RewardExp { get; set; }
    public List<int> RewardCurrencyIds { get; set; } = new();
    public List<int> RewardNums { get; set; } = new();

    public int TableID => Id;

    public bool IsSatisfied
    {
        get
        {
            bool result = true;
            for (int i = 0; i < AchievementIDs.Count; i++)
            {
                result &= GameManager.questManager.Achievements[AchievementIDs[i]] >= RequireNums[i];
            }
            return result;
        }
    }

    private readonly string formatAchievementID = "AchievementId{0}";
    private readonly string formatRequireNum = "RequireNum{0}";
    private readonly string formatRewardID = "RewardCurrencyId{0}";
    private readonly string formatRewardNum = "RewardNum{0}";

    public void ExtraLoad(CsvReader reader)
    {
        int count = 1;
        while (true)
        {
            bool isNoneData = true;
            if (reader.TryGetField<int>(string.Format(formatAchievementID, count), out var achieveID)
                && reader.TryGetField<int>(string.Format(formatRequireNum, count), out var requireNum))
            {
                if (achieveID != 0)
                {
                    AchievementIDs.Add(achieveID);
                    RequireNums.Add(requireNum);
                }
                isNoneData &= false;
            }
            else
            {
                isNoneData &= true;
            }

            if (reader.TryGetField<int>(string.Format(formatRewardID, count), out var rewardID)
                && reader.TryGetField<int>(string.Format(formatRewardNum, count), out var rewardNum))
            {
                if (rewardID != 0)
                {
                    RewardCurrencyIds.Add(rewardID);
                    RewardNums.Add(rewardNum);
                }
                isNoneData &= false;
            }
            else
            {
                isNoneData &= true;
            }

            if (isNoneData)
                break;

            count++;
        }
    }


    public void GetReward()
    {
        var itemList = GameManager.itemManager.ownItemList;

        GameManager.playerManager.Exp += RewardExp;
        for (int i = 0; i < RewardCurrencyIds.Count; i++)
        {
            if (itemList.ContainsKey(RewardCurrencyIds[i]))
            {
                itemList[RewardCurrencyIds[i]] += RewardNums[i];
            }
            else
            {
                itemList.Add(RewardCurrencyIds[i], RewardNums[i]);
            }
        }
    }
}