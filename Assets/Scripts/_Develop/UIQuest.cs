using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestDevelop : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI text;

    private void Awake()
    {
        GameManager.Subscribe(EVENT_TYPE.START, OnGameStart);
    }

    private void OnGameStart()
    {
        button.onClick.AddListener(OnButtonClick);
        var qm = GameManager.questManager;
        qm.OnAchievementUpdated += UpdateInfo;
        qm.OnQuestAccepted += UpdateInfo;
        qm.OnQuestSatisfied += UpdateInfo;
        qm.OnQuestCleared += UpdateInfo;
        UpdateInfo();
    }

    private void OnButtonClick()
    {
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
            SetText("올클리어");
            return;
        }

        var quest = qm.CurrentQuest;
        if(quest == null)
        {
            SetText("퀘스트 없음");
        }
        else if(quest.IsSatisfied)
        {
            SetText(quest.Name + " 클리어");
        }
        else
        {
            SetText(quest.Name);
        }

    }

    private void SetText(string text)
    {
        this.text.text = text;
    }
}