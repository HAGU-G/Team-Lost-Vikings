using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[JsonObject(MemberSerialization.OptIn)]
public class DialogManager
{
    public Queue<int> DialogQueue { get; private set; } = new();
    [JsonProperty] public List<int> ShowedDialog { get; private set; } = new();
    private List<DialogData> currentData = null;

    /// <summary>
    /// Ket: 다이얼로그 ID, Value: 퀘스트 데이터
    /// </summary>
    [JsonProperty] private Dictionary<int, QuestData> rewardList = new();


    public DialogData PrevScript { get; private set; }
    private int currentIndex;
    public DialogData CurrentScript { get; private set; }
    public DialogData NextScript { get; private set; }

    public event Action OnScriptChanged = null;
    public event Action OnDialogStart = null;
    public event Action OnDialogEnd = null;

    public bool IsShowing => currentData != null;
    public bool IsFirstScript => PrevScript == null;
    public bool IsLastScript => NextScript == null;

    public bool CanStart =>
        GameManager.IsReady
        && !GameManager.IsPlayingAnimation 
        && !IsShowing 
        && !GameManager.uiManager.windows[WINDOW_NAME.MESSAGE_POPUP].isOpened;

    public void Book(int id, bool doForceShow = false)
    {
        if (doForceShow)
        {
            Start(id);
            return;
        }

        if (!DialogQueue.Contains(id) && !ShowedDialog.Contains(id))
            DialogQueue.Enqueue(id);

        if (CanStart
            && DialogQueue.Count > 0)
        {
            Start(DialogQueue.Peek());
        }
    }

    public void BookReward(int dialogID, QuestData questData)
    {
        if (!rewardList.ContainsKey(dialogID))
            rewardList.Add(dialogID, questData);
    }

    public void Start(int id)
    {
        if (!CanStart)
            return;

        currentData = DataTableManager.dialogTable.GetData(id);
        OnDialogStart?.Invoke();
        SetCurrentScript(0);
    }

    private void SetCurrentScript(int index)
    {
        if (index < 0 || index >= currentData.Count)
            return;

        if (index > 0)
            PrevScript = currentData[index - 1];
        else
            PrevScript = null;

        currentIndex = index;
        CurrentScript = currentData[index];

        if (index < currentData.Count - 1)
            NextScript = currentData[index + 1];
        else
            NextScript = null;

        OnScriptChanged?.Invoke();
    }

    public void End()
    {
        var id = currentData[0].Id;
        if (rewardList.TryGetValue(id, out var quest))
        {
            quest.GetReward();
            rewardList.Remove(id);
        }

        if (!ShowedDialog.Contains(id))
            ShowedDialog.Add(id);

        OnDialogEnd?.Invoke();

        currentData = null;
        PrevScript = null;
        CurrentScript = null;
        NextScript = null;

        DialogQueue.Dequeue();

        if (CanStart && DialogQueue.Count > 0)
            Start(DialogQueue.Peek());
    }

    public void Next() => SetCurrentScript(currentIndex + 1);
    public void Prev() => SetCurrentScript(currentIndex - 1);
    public void ShowLastScript() => SetCurrentScript(currentData.Count - 1);


}