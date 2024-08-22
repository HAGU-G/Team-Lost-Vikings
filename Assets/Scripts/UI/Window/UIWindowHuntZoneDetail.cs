using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowHuntZoneDetail : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.HUNTZONE_DETAIL;

    [Header("텍스트")]
    public TextMeshProUGUI textMonsterType;
    public TextMeshProUGUI textHP;
    public TextMeshProUGUI textCombatPoint;
    public TextMeshProUGUI textPhysicalDef;
    public TextMeshProUGUI textMagicalDef;
    public TextMeshProUGUI textSpecialDef;

    private static readonly string formatDef = "{0}%";
    private static readonly string typeBoss = "보스 몬스터";
    private static readonly string typeNormal = "일반 몬스터";
    private static readonly string typeNull = "소환되는 몬스터 없음";

    [Header("버튼")]
    public Button buttonNormal;
    public Button buttonBoss;
    public Button buttonClose;

    [Header("프리펩")]
    public RawImage prefabRawImage;

    private int currentHuntZoneNum = -1;
    private UnitStats normalMonster = null;
    private UnitStats bossMonster = null;
    private UnitStats currentMonster = null;

    private HuntZoneManager hm = null;

    protected override void Awake()
    {
        base.Awake();
        GameManager.Subscribe(EVENT_TYPE.INIT, OnGameInit);

        buttonClose.onClick.AddListener(Close);

        buttonNormal.onClick.AddListener(ShowNormal);
        buttonBoss.onClick.AddListener(ShowBoss);
    }

    private void OnGameInit()
    {
        hm = GameManager.huntZoneManager;
        hm.OnHuntZoneInfoChanged += OnInfoChanged;
    }

    private void OnDestroy()
    {
        if (hm != null)
            hm.OnHuntZoneInfoChanged -= OnInfoChanged;
    }

    public override void Open()
    {
        base.Open();
        UpdateInfo(currentMonster);
    }

    private void OnInfoChanged()
    {
        SetHuntZoneNum(currentHuntZoneNum);
    }

    public void SetHuntZoneNum(int huntZoneNum)
    {
        if (hm == null || !hm.HuntZones.ContainsKey(huntZoneNum))
            return;

        var currentHuntZone = hm.HuntZones[huntZoneNum];

        //일반 몬스터 정보
        var normalData = currentHuntZone.GetCurrentMonster();
        if (normalData != null)
        {
            normalMonster ??= new();
            normalMonster.InitStats(normalData);
            normalMonster.ResetStats();
            normalMonster.isBoss = false;
        }
        else
        {
            normalMonster = null;
        }

        //보스 몬스터 정보
        var bossData = currentHuntZone.GetCurrentBoss();
        if (bossData != null)
        {
            bossMonster ??= new();
            bossMonster.InitStats(bossData);
            bossMonster.ResetStats();
            bossMonster.isBoss = true;
        }
        else
        {
            bossMonster = null;
        }

        currentMonster ??= normalMonster;
        UpdateInfo(currentMonster);
    }

    public void ShowNormal() => UpdateInfo(normalMonster);
    public void ShowBoss() => UpdateInfo(bossMonster);

    public void UpdateInfo(UnitStats stats)
    {
        if (!gameObject.activeSelf)
            return;

        currentMonster = stats;

        if (stats == null)
        {
            textMonsterType.text = typeNull;
            textHP.text = string.Empty;
            textCombatPoint.text = string.Empty;
            textPhysicalDef.text = string.Empty;
            textMagicalDef.text = string.Empty;
            textSpecialDef.text = string.Empty;

            buttonNormal.interactable = normalMonster != null;
            buttonBoss.interactable = bossMonster != null;

            prefabRawImage.enabled = false;
        }
        else
        {
            textMonsterType.text = stats.isBoss ? typeBoss : typeNormal;
            textHP.text = stats.HP.max.ToString();
            textCombatPoint.text = stats.CombatPoint.ToString();
            textPhysicalDef.text = string.Format(formatDef, stats.PhysicalDef.Current);
            textMagicalDef.text = string.Format(formatDef, stats.MagicalDef.Current);
            textSpecialDef.text = string.Format(formatDef, stats.SpecialDef.Current);

            buttonNormal.interactable = stats.isBoss && normalMonster != null;
            buttonBoss.interactable = !stats.isBoss && bossMonster != null;

            prefabRawImage.enabled = true;
            var rect = GameManager.uiManager.unitRenderTexture.LoadRenderTexture(stats.Data.UnitAssetFileName);
            prefabRawImage.uvRect = rect;
        }
    }
}