﻿using UnityEngine;
using UnityEngine.UI;

public class BuildComplete : MonoBehaviour
{
    public Button yesButton;
    public Button noButton;

    private void Awake()
    {
        GameManager.Subscribe(EVENT_TYPE.START, OnGameStart);
        gameObject.SetActive(false);
    }

    public void OnGameStart()
    {
        yesButton.onClick.AddListener(OnButtonYes);
        noButton.onClick.AddListener(OnButtonNo);
    }

    private void OnButtonYes()
    {
        GameManager.PlayButtonSFX();
        var constructMode = GameManager.uiManager.windows[WINDOW_NAME.CONSTRUCT_MODE] as UIConstructMode;
        if(constructMode.isConstructing)
            constructMode.ConstructDecide();
        if (constructMode.IsReplacing)
            constructMode.ReplaceDecide();
        GameManager.villageManager.construct.ResetPrevTileColor();
        gameObject.SetActive(false);
    }

    private void OnButtonNo()
    {
        GameManager.PlayButtonSFX();
        var constructMode = GameManager.uiManager.windows[WINDOW_NAME.CONSTRUCT_MODE] as UIConstructMode;
        constructMode.ConstructCancel();
        GameManager.villageManager.construct.ResetPrevTileColor();
        gameObject.SetActive(false);
    }

}
