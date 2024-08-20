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
        GameManager.Subscribe(EVENT_TYPE.START, OnGameStart);
    }

    private void OnGameStart()
    {
        yesButton.onClick.AddListener(OnButtonYes);
        noButton.onClick.AddListener(OnButtonNo);
    }

    private void OnButtonYes()
    {

    }

    private void OnButtonNo()
    { 
        gameObject.SetActive(false);
    }
}
