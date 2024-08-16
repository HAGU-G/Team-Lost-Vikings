using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogDevelop : MonoBehaviour
{
    public Button buttonSkip;
    public Button buttonNext;
    public Button buttonPrev;

    public TextMeshProUGUI textNextButton;
    public TextMeshProUGUI prev;
    public TextMeshProUGUI current;
    public TextMeshProUGUI next;

    private void Awake()
    {
        GameManager.Subscribe(EVENT_TYPE.START, OnGameStart);
    }

    private void OnGameStart()
    {
        var dm = GameManager.dialogManager;
        buttonPrev.onClick.AddListener(dm.Prev);
        buttonSkip.onClick.AddListener(dm.End);
        buttonNext.onClick.AddListener(OnClickNext);

        dm.OnScriptChanged += UpdateInfo;
        dm.OnDialogEnd += () => gameObject.SetActive(false);
        dm.OnDialogStart += () => gameObject.SetActive(true);

        gameObject.SetActive(false);
    }

    private void OnClickNext()
    {
        var dm = GameManager.dialogManager;
        if (dm.IsLastScript)
            dm.End();
        else
            dm.Next();
    }

    private void UpdateInfo()
    {
        var dm = GameManager.dialogManager;

        //이전
        buttonPrev.gameObject.SetActive(!dm.IsFirstScript);
        if (dm.IsFirstScript)
            prev.text = string.Empty;
        else
            prev.text = dm.PrevScript.SpeakerName + "\n" + dm.PrevScript.DialogText;

        //현재
        if (dm.IsShowing)
            current.text = dm.CurrentScript.SpeakerName + "\n" + dm.CurrentScript.DialogText;

        //다음
        if (dm.IsLastScript)
            next.text = string.Empty;
        else
            next.text = dm.NextScript.SpeakerName + "\n" + dm.NextScript.DialogText;

        //스킵
        switch (dm.CurrentScript.SkipBtnEnable)
        {
            case BUTTON_SHOW_TYPE.NONE:
                buttonSkip.gameObject.SetActive(false);
                break;
            case BUTTON_SHOW_TYPE.ACTIVE:
                buttonSkip.gameObject.SetActive(true);
                buttonSkip.interactable = true;
                break;
            case BUTTON_SHOW_TYPE.DISABLE:
                buttonSkip.gameObject.SetActive(true);
                buttonSkip.interactable = false;
                break;
        }
    }
}