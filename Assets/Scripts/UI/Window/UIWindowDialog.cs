using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class UIWindowDialog : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.DIALOG;

    public Button buttonSkip;
    public Button buttonNext;
    public Button buttonPrev;

    public TextMeshProUGUI textNextButton;
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textScript;

    public Image imageLeft;
    public Image imageRight;

    private List<AsyncOperationHandle<Sprite>> handles = new();
    private Dictionary<string, Sprite> loadedSprites = new();


    protected override void Awake()
    {
        base.Awake();
    }

    private void OnDestroy()
    {
        foreach (var handle in handles)
        {
            Addressables.Release(handle);
        }
        handles.Clear();
    }

    protected override void OnGameStart()
    {
        base.OnGameStart();
        var dm = GameManager.dialogManager;
        buttonPrev.onClick.AddListener(dm.Prev);
        buttonSkip.onClick.AddListener(dm.End);
        buttonNext.onClick.AddListener(OnClickNext);
        buttonPrev.onClick.AddListener(GameManager.PlayButtonSFX);
        buttonSkip.onClick.AddListener(GameManager.PlayButtonSFX);
        buttonNext.onClick.AddListener(GameManager.PlayButtonSFX);

        dm.OnScriptChanged += UpdateInfo;
        dm.OnDialogStart += Open;
        dm.OnDialogEnd += Close;

        gameObject.SetActive(false);
    }

    public override void Open()
    {
        var prevState = isOpened;
        base.Open();
        if (prevState != isOpened && GameManager.IsReady)
            GameManager.GamePause();
    }

    public override void Close()
    {
        var prevState = isOpened;
        base.Close();
        if (prevState != isOpened && GameManager.IsReady)
            GameManager.GameResume();
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

        //이전 버튼
        buttonPrev.gameObject.SetActive(!dm.IsFirstScript);

        //스크립트 & 이미지
        if (dm.IsShowing)
        {
            textName.text = dm.CurrentScript.SpeakerName;
            textScript.text = dm.CurrentScript.DialogText;

            var imageName = dm.CurrentScript.ImageFileName;
            if (loadedSprites.ContainsKey(imageName))
            {
                //이미 로드를 시작함. 아무행동 하지 않음.
            }
            else if (Addressables.LoadResourceLocationsAsync(imageName).WaitForCompletion().Count <= 0)
            {
                if (!(imageName == "0" || imageName == string.Empty))
                {
                    Debug.Log($"{imageName} 로드 실패");
                }
            }
            else
            {
                loadedSprites.Add(imageName, null);
                var handle = Addressables.LoadAssetAsync<Sprite>(imageName);
                if (!handles.Contains(handle))
                    handles.Add(handle);
                handle.Completed += (x) =>
                {
                    if (x.Status != AsyncOperationStatus.Succeeded)
                    {
                        loadedSprites.Remove(imageName);
                        handles.Remove(handle);
                        Addressables.Release(handle);
                        return;
                    }
                    loadedSprites[imageName] = x.Result;
                    UpdateImage();
                };
            }
            UpdateImage();
        }

        //스킵 버튼
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

    private void UpdateImage()
    {
        var dm = GameManager.dialogManager;

        Sprite sprite = null;
        loadedSprites.TryGetValue(dm.CurrentScript.ImageFileName, out sprite);

        switch (dm.CurrentScript.ImageMarker)
        {
            case DIRECTION_HORIZENTAL.NONE:
                imageLeft.enabled = false;
                imageRight.enabled = false;
                break;
            case DIRECTION_HORIZENTAL.LEFT:
                imageLeft.sprite = sprite;
                imageLeft.enabled = sprite != null;
                imageRight.enabled = false;
                break;
            case DIRECTION_HORIZENTAL.RIGHT:
                imageRight.sprite = sprite;
                imageRight.enabled = sprite != null;
                imageLeft.enabled = false;
                break;
            case DIRECTION_HORIZENTAL.BOTH:
                imageLeft.sprite = sprite;
                imageRight.sprite = sprite;
                imageLeft.enabled = sprite != null;
                imageRight.enabled = sprite != null;
                break;
        }

    }
}