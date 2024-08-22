using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuest : MonoBehaviour
{
    public UIAchievementInfo[] achievements;
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textDesc;

    public Button buttonClear;
    public TextMeshProUGUI textButtonClear;

    public static readonly string formatName = "<{0}>";
    public static readonly string formatAchieveDesc = "{0:0}/{1:0}";
    public static readonly string stringAllClear = "모든 가이드 퀘스트를\n클리어했습니다.";
    public static readonly string stringClear = "퀘스트 클리어\n보상 획득";

    private void Awake()
    {
        GameManager.Subscribe(EVENT_TYPE.START, OnGameStart);
    }

    private void OnGameStart()
    {
        buttonClear.onClick.AddListener(OnButtonClick);

        var qm = GameManager.questManager;
        qm.OnAchievementUpdated += UpdateInfo;
        qm.OnQuestAccepted += UpdateInfo;
        qm.OnQuestSatisfied += UpdateInfo;
        qm.OnQuestCleared += UpdateInfo;
        UpdateInfo();
    }

    private void OnButtonClick()
    {
        buttonClear.onClick.AddListener(GameManager.PlayButtonSFX);

        var qm = GameManager.questManager;
        if (qm.IsAllClear)
            return;

        if (qm.CurrentQuest == null)
            return;

        if (qm.CurrentQuest.IsSatisfied)
            GameManager.questManager.QuestClear(GameManager.questManager.CurrentQuest.Id);

    }

    private void UpdateInfo()
    {
        var qm = GameManager.questManager;

        if (qm.IsAllClear)
        {
            SetText(string.Format(formatName, "가이드퀘스트"), string.Empty);
            foreach (var achieve in achievements)
            {
                achieve.gameObject.SetActive(false);
            }
            buttonClear.gameObject.SetActive(true);
            buttonClear.interactable = false;
            return;
        }

        var quest = qm.CurrentQuest;
        if (quest == null)
        {
            SetText(string.Format(formatName, "퀘스트 없음"), string.Empty);
            foreach (var achieve in achievements)
            {
                achieve.gameObject.SetActive(false);
            }
            buttonClear.gameObject.SetActive(false);
        }
        else
        {
            SetText(string.Format(formatName, quest.Name), quest.QuestDesc);
            for (int i = 0; i < achievements.Length; i++)
            {
                if (quest.AchievementIDs.Count <= i)
                {
                    achievements[i].gameObject.SetActive(false);
                    continue;
                }

                var id = quest.AchievementIDs[i];
                if (!qm.Achievements.ContainsKey(id))
                {
                    achievements[i].gameObject.SetActive(false);
                    continue;
                }

                achievements[i].textName.text = DataTableManager.achievementTable.GetData(id).AchieveName;
                achievements[i].textDesc.text = string.Format(
                    formatAchieveDesc,
                    qm.Achievements[id],
                    qm.CurrentQuest.RequireNums[i]);
                achievements[i].gameObject.SetActive(true);

            }
            buttonClear.gameObject.SetActive(false);
        }

        if (quest.IsSatisfied)
        {
            buttonClear.gameObject.SetActive(quest.IsSatisfied);
            textButtonClear.text = stringClear;
        }

    }

    private void SetText(string name, string desc)
    {
        textName.text = name;
        textDesc.text = desc;
    }
}