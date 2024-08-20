using UnityEngine;
using UnityEngine.UI;

public class ResetPopUp : MonoBehaviour
{
    public Button yesButton;
    public Button noButton;

    private void Awake()
    {
        yesButton.onClick.AddListener(OnButtonYes);
        noButton.onClick.AddListener(OnButtonNo);
        //GameManager.Subscribe(EVENT_TYPE.START, OnGameStart);
    }

    private void OnGameStart()
    {

        Debug.Log("ongamestart");
        
    }

    private void OnButtonYes()
    {
        var remove = SaveManager.RemoveSaveFile();
        GameManager.IsReady = false;
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnButtonNo()
    {
        gameObject.SetActive(false);
    }
}
