using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHuntZoneInfomation : MonoBehaviour
{
    public TextMeshProUGUI textHuntZoneName;
    public TextMeshProUGUI textUnitDeployCount;
    public static readonly string formatDeployCount = "{0}/{1}";

    public Button buttonDetailInfo;
    public Button buttonName;

    private CameraManager cm = null;
    private HuntZoneManager hm = null;
    private UIManager um = null;

    private void Awake()
    {
        GameManager.Subscribe(EVENT_TYPE.INIT, OnGameInit);
        buttonDetailInfo.onClick.AddListener(ShowDetailInfo);
        buttonName.onClick.AddListener(StageDown);

        buttonDetailInfo.onClick.AddListener(GameManager.PlayButtonSFX);
        buttonName.onClick.AddListener(GameManager.PlayButtonSFX);
    }

    private void OnGameInit()
    {
        cm = GameManager.cameraManager;
        cm.OnLocationChanged += UpdateInfo;

        hm = GameManager.huntZoneManager;
        hm.OnDeploymentChaneged += UpdateInfo;
        hm.OnHuntZoneInfoChanged += UpdateInfo;

        um = GameManager.uiManager;
    }

    private void OnDestroy()
    {
        if (cm != null)
            cm.OnLocationChanged -= UpdateInfo;
        if (hm != null)
        {
            hm.OnDeploymentChaneged -= UpdateInfo;
            hm.OnHuntZoneInfoChanged -= UpdateInfo;
        }
    }

    public void UpdateInfo()
    {
        if (hm == null || cm == null)
            return;

        var currentHuntZoneNum = cm.HuntZoneNum;

        gameObject.SetActive(
            hm.HuntZones.ContainsKey(currentHuntZoneNum)
            && cm.LookLocation == LOCATION.HUNTZONE);

        if (!gameObject.activeSelf)
            return;

        var currentData = hm.HuntZones[currentHuntZoneNum].GetCurrentData();

        textHuntZoneName.text = currentData.HuntZoneName;
        textUnitDeployCount.text =
             string.Format(
                formatDeployCount,
                hm.UnitDeployment[currentHuntZoneNum].Count,
                currentData.UnitCapacity);
    }

    public void ShowDetailInfo()
    {
        var windowDetail = um.windows[WINDOW_NAME.HUNTZONE_DETAIL] as UIWindowHuntZoneDetail;

        windowDetail.SetHuntZoneNum(cm.HuntZoneNum);
        windowDetail.Open();
    }

    private void StageDown()
    {
        if (hm == null || cm == null)
            return;

        var currentHuntZone = cm.HuntZoneNum;
        if (!hm.HuntZones.ContainsKey(currentHuntZone)
            || hm.HuntZones[currentHuntZone].Stage <= 1)
            return;

        var message = um.windows[WINDOW_NAME.MESSAGE_POPUP] as UIWindowMessage;
        message.ShowMessage(
            $"스테이지를 내립니다.\n다시 보스를 잡아야 현재 스테이지로\n돌아올 수 있습니다.",
            true,
            onConfirmButtonClick: ()=>
            {
                hm.HuntZones[currentHuntZone].SetStage(hm.HuntZones[currentHuntZone].Stage - 1);
            },
            onCancelButtonClick: () => { }
            );
    }
}
