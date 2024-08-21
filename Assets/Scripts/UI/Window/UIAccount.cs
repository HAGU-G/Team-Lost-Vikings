using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAccount : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.ACCOUNT;

    public Button option;
    public Button reset;
    public Button gameQuit;
    public Button close;
    public GameObject resetPopup;

    protected override void OnGameStart()
    {
        base.OnGameStart();

        resetPopup.SetActive(false);
        option.onClick.AddListener(OnButtonOption);
        reset.onClick.AddListener(OnButtonReset);
        gameQuit.onClick.AddListener(OnButtonGameQuit);
        close.onClick.AddListener(OnButtonClose);
    }

    private void OnButtonOption()
    {
        GameManager.uiManager.windows[WINDOW_NAME.OPTION].Open();
    }

    private void OnButtonReset()
    {
        resetPopup.SetActive(true);
    }

    private void OnButtonGameQuit()
    {
        var constructMode = GameManager.uiManager.windows[WINDOW_NAME.CONSTRUCT_MODE] as UIConstructMode;
        if (GameManager.villageManager.constructMode.isConstructMode)
        {
            constructMode.FinishConstructMode();
            GameManager.Publish(EVENT_TYPE.CONSTRUCT);
        }

        GameManager.GameQuit();
    }

    private void OnButtonClose()
    {
        Close();
    }
}
