using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIOption : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.OPTION;

    public Button account;
    public Button frame_30;
    public Button frame_60;

    public Slider bgmBar;
    public Slider sfxBar;

    public TMP_InputField bgmInputField;
    public TMP_InputField sfxInputField;

    public Button gameQuit;
    public Button close;


    protected override void OnGameStart()
    {
        base.OnGameStart();

        account.onClick.AddListener(OnButtonAccount);
        frame_30.onClick.AddListener(OnButtonFrame30);
        frame_60.onClick.AddListener(OnButtonFrame60);
        gameQuit.onClick.AddListener(OnButtonGameQuit);
        close.onClick.AddListener(OnButtonClose);

        bgmInputField.keyboardType = TouchScreenKeyboardType.NumberPad;
        bgmInputField.contentType = TMP_InputField.ContentType.IntegerNumber;
        bgmBar.onValueChanged.AddListener(OnBGMBarChanged);
        bgmInputField.onSubmit.AddListener(OnButtonBGMInput);

        sfxInputField.keyboardType = TouchScreenKeyboardType.NumberPad;
        sfxInputField.contentType = TMP_InputField.ContentType.IntegerNumber;
        sfxBar.onValueChanged.AddListener(OnSFXBarChanged);
        sfxInputField.onSubmit.AddListener(OnButtonSFXInput);

    }

    private void OnEnable()
    {
        bgmBar.value = SoundManager.BGMVolume;
        bgmInputField.text = Mathf.FloorToInt(SoundManager.BGMVolume * 100).ToString();
        sfxBar.value = SoundManager.SFXVolume;
        sfxInputField.text = Mathf.FloorToInt(SoundManager.SFXVolume * 100).ToString();
    }

    private void OnButtonBGMInput(string value)
    {
        if (float.TryParse(value, out float volume))
        {
            volume = Mathf.Clamp(volume, 0, 100);
            if (Mathf.RoundToInt(bgmBar.value * 100) != Mathf.RoundToInt(volume))
            {
                bgmBar.value = volume / 100f;
            }
            SoundManager.BGMVolume = volume / 100f;
        }
    }

    private void OnBGMBarChanged(float value)
    {
        var volume = Mathf.RoundToInt(value * 100);
        bgmInputField.text = volume.ToString();
        SoundManager.BGMVolume = value;
    }

    private void OnButtonSFXInput(string value)
    {
        if (float.TryParse(value, out float volume))
        {
            volume = Mathf.Clamp(volume, 0, 100);
            if (Mathf.RoundToInt(sfxBar.value * 100) != Mathf.RoundToInt(volume))
            {
                sfxBar.value = volume / 100f;
            }
            SoundManager.SFXVolume = volume / 100f;
        }
    }

    private void OnSFXBarChanged(float value)
    {
        var volume = Mathf.RoundToInt(value * 100);
        sfxInputField.text = volume.ToString();
        SoundManager.SFXVolume = value;
    }

    private void OnButtonAccount()
    {
        GameManager.uiManager.windows[WINDOW_NAME.ACCOUNT].Open();
    }

    private void OnButtonFrame30()
    {
        Application.targetFrameRate = 30;
    }

    private void OnButtonFrame60()
    {
        Application.targetFrameRate = 60;
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
