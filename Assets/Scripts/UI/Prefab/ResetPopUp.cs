using System.Collections;
using System.Collections.Generic;
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
        Debug.Log("yes");
        var remove = SaveManager.RemoveSaveFile();
        GameManager.IsReady = false;
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnButtonNo()
    {
        Debug.Log("no");
        gameObject.SetActive(false);
    }
}
