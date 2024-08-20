using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class ItemManager
{
    //[JsonProperty] private int _gold = 1000;
    //public int Gold
    //{
    //    get
    //    {
    //        var goldID = GameSetting.Instance.goldID;
    //        if (!ownItemList.TryGetValue(goldID, out int gold))
    //        {
    //            ownItemList.Add(goldID, 0);
    //            return ownItemList[goldID];
    //        }
    //        return ownItemList[goldID];
    //    }
    //    set
    //    {
    //        var goldID = GameSetting.Instance.goldID;
    //        if (!ownItemList.TryGetValue(goldID, out int gold))
    //        {
    //            ownItemList.Add(goldID, 0);
    //            ownItemList[goldID] = value;
    //        }

    //        if (value >= goldLimit)
    //        {
    //            if (ownItemList[goldID] < goldLimit)
    //            {
    //                ownItemList[goldID] = goldLimit;
    //            }
    //        }
    //        else
    //        {
    //            ownItemList[goldID] = value;
    //        }


    //        GameManager.uiManager.uiDevelop.SetGold(ownItemList[goldID]);
    //    }
    //}
    public int Gold
    {
        get
        {
            return GetItem(GameSetting.Instance.goldID);
        }
        set
        {
            var goldID = GameSetting.Instance.goldID;
            if (!ownItemList.ContainsKey(goldID))
                ownItemList.Add(goldID, 0);

            var deltaAmount = value - ownItemList[goldID];
            if (deltaAmount > 0)
                AddItem(goldID, deltaAmount);
            else if (deltaAmount < 0)
                SpendItem(goldID, -deltaAmount, true);
        }
    }
    [JsonProperty] public int Rune { get; set; }

    [JsonProperty] public Dictionary<int, int> ownItemList = new();
    public int goldLimit = 4000;

    public delegate void OnItemChanged();
    public event OnItemChanged OnItemChangedCallback;

    public int GetItem(int id)
    {
        if (ownItemList.TryGetValue(id, out int amount))
            return amount;
        else
            return 0;
    }

    public void AddItem(int id, int amount)
    {
        if (amount < 0)
            return;

        if (!ownItemList.ContainsKey(id))
            ownItemList.Add(id, 0);

        var prevAmount = ownItemList[id];
        var afterAmount = ownItemList[id] + amount;
        switch (id)
        {
            case int goldID when goldID == GameSetting.Instance.goldID:
                if (afterAmount > goldLimit)
                    ownItemList[id] = Mathf.Max(goldLimit, ownItemList[id]);
                else
                    ownItemList[id] = afterAmount;
                break;
            default:
                ownItemList[id] = afterAmount;
                GameManager.uiManager.uiDevelop.SetItem(id, ownItemList[id]);
                break;
        }

        GameManager.questManager.SetAchievementCountByTargetID(
            id,
            ACHIEVEMENT_TYPE.ITEM_GET,
            ownItemList[id] - prevAmount);

        OnItemChangedCallback?.Invoke();
    }

    public bool SpendItem(int id, int amount, bool doForceSpend = false)
    {
        if (!doForceSpend
            && (amount < 0
                || !ownItemList.ContainsKey(id)
                || ownItemList[id] - amount < 0))
            return false;

        ownItemList[id] -= amount;

        switch (id)
        {
            case int goldID when goldID == GameSetting.Instance.goldID:
                break;
            default:
                GameManager.uiManager.uiDevelop.SetItem(id, ownItemList[id]);
                break;
        }

        OnItemChangedCallback?.Invoke();

        GameManager.questManager.SetAchievementCountByTargetID(id, ACHIEVEMENT_TYPE.ITEM_USE, amount);
        return true;
    }

    //public bool AddGold(int amount)
    //{
    //    if (!ownItemList.TryGetValue(0, out int gold))
    //    {
    //        ownItemList.Add(0, 0);
    //    }

    //    if (ownItemList[0] + amount >= goldLimit)
    //    {
    //        if (ownItemList[0] < goldLimit)
    //        {
    //            ownItemList[0] = goldLimit;
    //        }
    //        return false;
    //    }

    //    ownItemList[0] += amount;
    //    return true;
    //}

    //public bool SpendGold(int amount)
    //{
    //    if (!ownItemList.TryGetValue(0, out int gold))
    //    {
    //        ownItemList.Add(0, 0);
    //    }

    //    if (ownItemList[0] - amount < 0)
    //    {
    //        return false;
    //    }

    //    ownItemList[0] -= amount;
    //    return true;
    //}
}
