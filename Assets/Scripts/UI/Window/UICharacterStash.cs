using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class UICharacterStash : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.CHARACTER_STASH;

    public Button exit;
    public Button goOwnUnit;

    public TextMeshProUGUI ownUnitCount;

    public Transform content;
    public GameObject unitInfo;
    private Dictionary<int, Sprite> gradeIcons = new();

    private List<GameObject> infos = new();

    private int unitCnt = 0;

    public void OnButtonExit()
    {
        Close();
    }

    private void Update()
    {
        if (unitCnt != GameManager.unitManager.Waitings.Count)
            LoadCharacterButtons(GameManager.unitManager.Waitings);
    }

    private void OnEnable()
    {
        if (!IsReady)
            return;

        LoadCharacterButtons(GameManager.unitManager.Waitings);
    }
    protected override void OnGameStart()
    {
        base.OnGameStart();

        var cnt = Enum.GetValues(typeof(UNIT_GRADE)).Length;
        for (int i = 0; i < cnt; ++i)
        {
            var path = $"Grade_0{i+1}";
            var id = i;
            Addressables.LoadAssetAsync<Sprite>(path).Completed += (obj) => OnLoadDone(obj, id);
        }

        exit.onClick.AddListener(OnButtonExit);
        goOwnUnit.onClick.AddListener(OnButtonOwnUnit);
    }

    private void OnLoadDone(AsyncOperationHandle<Sprite> obj, int id)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            gradeIcons.Add(id, obj.Result);
        }
    }

    public void LoadCharacterButtons(Dictionary<int, UnitStats> units)
    {
        for (int i = 0; i < infos.Count; ++i)
        {
            Destroy(infos[i]);
        }
        infos.Clear();

        unitCnt = units.Count;

        foreach (var unit in units)
        {
            var obj = GameObject.Instantiate(unitInfo, content);
            var info = obj.GetComponent<CharacterInfo>();
            info.characterName.text = $"{unit.Value.Data.Name}";
            info.gradeIcon.sprite = gradeIcons.GetValueOrDefault((int)unit.Value.UnitGrade);
            info.characterIcon.uvRect
                = GameManager.uiManager.unitRenderTexture.LoadRenderTexture(unit.Value.Data.UnitAssetFileName);
            info.information.onClick.AddListener(
            () =>
            {
                GameManager.uiManager.currentUnitStats = unit.Value;
                GameManager.uiManager.windows[WINDOW_NAME.UNIT_DETAIL_INFORMATION].Open();
            }
                );
            info.location_state.text = $"대기소 - 대기 중";
            info.characterId = unit.Value.InstanceID;
            infos.Add(obj);
        }

        ownUnitCount.text = $"{infos.Count} / {GameSetting.Instance.waitListLimit}";
    }

    private void OnButtonOwnUnit()
    {
        GameManager.uiManager.windows[WINDOW_NAME.UNITS_INFORMATION].Open();
    }
}
