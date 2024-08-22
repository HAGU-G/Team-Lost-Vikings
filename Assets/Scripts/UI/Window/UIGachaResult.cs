using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

public class UIGachaResult : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.GACHA_RESULT;

    public TextMeshProUGUI gachaResultDesc;
    public Image gradeICon;
    public RawImage unitImage;
    public TextMeshProUGUI unitName;
    public Button information;
    public Button reRecruit;

    public Button exit;
    private UnitStats resultUnit = new();

    private Dictionary<int, Sprite> gradeIcons = new();

    private void OnEnable()
    {
        if (!IsReady)
            return;
    }
    protected override void OnGameStart()
    {
        base.OnGameStart();

        var cnt = Enum.GetValues(typeof(UNIT_GRADE)).Length;
        for (int i = 0; i < cnt; ++i)
        {
            var path = $"Grade_0{i + 1}";
            var id = i;
            Addressables.LoadAssetAsync<Sprite>(path).Completed += (obj) => OnLoadDone(obj, id);
        }
        information.onClick.AddListener(OnButtonInformation);
        exit.onClick.AddListener(OnButtonExit);
    }

    private void OnLoadDone(AsyncOperationHandle<Sprite> obj, int id)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            gradeIcons.Add(id, obj.Result);
        }
    }

    public void SetResult(UnitStats unitStats)
    {
        SetUnit(unitStats);

        foreach (var u in GameManager.unitManager.Units.Values)
        {
            if (u.Id == unitStats.Id)
            {
                gachaResultDesc.text = $"보유 중인 모험가가 모집되어 대기소에 배치되었습니다.";
                reRecruit.GetComponentInChildren<TextMeshProUGUI>().text = "대기소로";
                reRecruit.onClick.RemoveAllListeners();
                reRecruit.onClick.AddListener(
                    () => GameManager.uiManager.windows[WINDOW_NAME.CHARACTER_STASH].Open());
            }
        }

        if (GameManager.unitManager.unitLimitCount < GameManager.unitManager.Units.Count + 1)
        {
            gachaResultDesc.text = $"자리가 없어서 대기소에 배치되었습니다.";
            reRecruit.GetComponentInChildren<TextMeshProUGUI>().text = "대기소로";
            reRecruit.onClick.RemoveAllListeners();
            reRecruit.onClick.AddListener(
                () => GameManager.uiManager.windows[WINDOW_NAME.CHARACTER_STASH].Open());
        }
        else
        {
            gachaResultDesc.text = $"새로운 모험가가 합류했습니다."; 
            reRecruit.GetComponentInChildren<TextMeshProUGUI>().text = "다시 모집";
            reRecruit.onClick.RemoveAllListeners();
            reRecruit.onClick.AddListener(
                () => GameManager.uiManager.windows[WINDOW_NAME.GACHA_UI].Open());
        }

        unitImage.uvRect = GameManager.uiManager.unitRenderTexture.LoadRenderTexture(unitStats.Data.UnitAssetFileName);
        gradeICon.sprite = gradeIcons.GetValueOrDefault((int)unitStats.UnitGrade);
        unitName.text = unitStats.Data.Name;
    }

    private void SetUnit(UnitStats unit)
    {
        resultUnit = unit;
    }

    private void OnButtonInformation()
    {
        GameManager.uiManager.currentUnitStats = resultUnit;
        GameManager.uiManager.windows[WINDOW_NAME.UNIT_DETAIL_INFORMATION].Open();
    }

    public void OnButtonExit()
    {
        Close();
    }
}
