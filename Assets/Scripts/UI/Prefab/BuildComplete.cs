using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        var constructMode = GameManager.uiManager.windows[WINDOW_NAME.CONSTRUCT_MODE] as UIConstructMode;
        constructMode.ConstructDecide();
        GameManager.villageManager.construct.ResetPrevTileColor();
        gameObject.SetActive(false);
    }

    private void OnButtonNo()
    {
        var constructMode = GameManager.uiManager.windows[WINDOW_NAME.CONSTRUCT_MODE] as UIConstructMode;
        constructMode.ConstructCancel();
        GameManager.villageManager.construct.ResetPrevTileColor();
        gameObject.SetActive(false);
    }

}
