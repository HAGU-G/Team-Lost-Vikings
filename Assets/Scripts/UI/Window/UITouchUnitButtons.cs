using TMPro;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using System.Collections.Generic;

public class UITouchUnitButtons : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.TOUCH_UNIT_BUTTONS;

    public Image gradeImage;
    public TextMeshProUGUI unitName;

    public Slider hpBar;
    public Slider staminaBar;
    public Slider mentalBar;

    public Button information;
    public Button close;
    public Button placement;

    private UIManager um;
    private string path = "Assets/Pick_Asset/2WEEK/GradeIcon/Grade_";
    private List<Sprite> gradeIcons = new();

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnGameStart()
    {
        base.OnGameStart();
        isShowOnly = false;

        um = GameManager.uiManager;

        for(int i = 1; i <= 5; ++i)
        {
            var newpath = $"{path}{i}.png";
            Addressables.LoadAssetAsync<Sprite>(newpath).Completed += OnLoadDone;
        }

        information.onClick.AddListener(OnButtonInformation);
        close.onClick.AddListener(OnButtonClose);
        placement.onClick.AddListener(OnButtonPlacement);
    }

    private void OnLoadDone(AsyncOperationHandle<Sprite> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            gradeIcons.Add(obj.Result);
        }
    }

    private void OnEnable()
    {
        SetUI();
    }

    private void SetUI()
    {
        SetText();
        SetGradeIcon();
    }

    private void SetText()
    {
        unitName.text = um.currentUnitStats.Data.Name;
    }

    private void SetGradeIcon()
    {
        gradeImage.sprite = gradeIcons[(int)um.currentUnitStats.UnitGrade];
    }

    private void SetParameterBar()
    {
        var stats = um.currentUnitStats;
        hpBar.value = (float)stats.HP.Current / (float)stats.HP.max;
        staminaBar.value = (float)stats.Stamina.Current / (float)stats.Stamina.max;
        mentalBar.value = (float)stats.Stress.Current / (float)stats.Stress.max;
    }

    private void Update()
    {
        if (um.currentUnitStats.isDead)
        {
            OnButtonClose();
            GameManager.cameraManager.SetLocation(LOCATION.VILLAGE);
        }

        SetParameterBar();
    }

    private void OnButtonInformation()
    {
        GameManager.PlayButtonSFX();
        GameManager.uiManager.windows[WINDOW_NAME.UNIT_DETAIL_INFORMATION].Open();
    }

    private void OnButtonClose()
    {
        GameManager.PlayButtonSFX();
        GameManager.cameraManager.FinishFocusOnUnit();
        Close();
    }

    private void OnButtonPlacement()
    {
        GameManager.PlayButtonSFX();
        GameManager.uiManager.windows[WINDOW_NAME.CHARACTER_LOCATE].Open();
    }
}