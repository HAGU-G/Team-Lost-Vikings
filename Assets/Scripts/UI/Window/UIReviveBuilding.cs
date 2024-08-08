using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIReviveBuilding : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.REVIVE_POPUP;

    public TextMeshProUGUI buildingName;
    public TextMeshProUGUI buildingDesc;
    public Transform reviveTransform;
    public GameObject revivePrefab;
    public TextMeshProUGUI reviveTimeText;
    public Transform upgradeTransform;
    public GameObject upgradePrefab;
    public Button upgrade;
    public Button exit;

    private UIManager um;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnGameStart()
    {
        base.OnGameStart();
        um = GameManager.uiManager;

        upgrade.onClick.AddListener(OnButtonUpgrade);
        exit.onClick.AddListener(OnButtonExit);
    }

    private void OnEnable()
    {
        SetUI();
    }

    private void SetUI()
    {
        buildingName.text = um.currentNormalBuidling.StructureName;
        buildingDesc.text = um.currentNormalBuidling.StructureDesc;

        SetRevivingList();

        var time = um.currentNormalBuidling.gameObject.GetComponent<ReviveBuilding>().reviveTime;
        reviveTimeText.text = $"부활 대기시간 {time}초";

        SetUpgradeItemList();
    }

    private void SetRevivingList()
    {

    }

    private void SetUpgradeItemList()
    {

    }

    public void OnButtonUpgrade()
    {

    }

    public void OnButtonExit()
    {
        Close();
    }
}