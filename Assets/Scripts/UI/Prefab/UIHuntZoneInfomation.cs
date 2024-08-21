using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHuntZoneInfomation : MonoBehaviour
{
    public TextMeshProUGUI textHuntZoneName;
    public TextMeshProUGUI textUnitDeployCount;
    public static readonly string formatDeployCount = "{0}/{1}";

    public Button buttonDetailInfo;

    private CameraManager cm = null;
    private HuntZoneManager hm = null;

    private void Awake()
    {
        GameManager.Subscribe(EVENT_TYPE.INIT, OnGameInit);
        buttonDetailInfo.onClick.AddListener(ShowDetailInfo);
    }

    private void OnGameInit()
    {
        cm = GameManager.cameraManager;
        cm.OnLocationChanged += UpdateInfo;

        hm = GameManager.huntZoneManager;
        hm.OnDeploymentChaneged += UpdateInfo;
    }

    private void OnDestroy()
    {
        if (cm != null)
            cm.OnLocationChanged -= UpdateInfo;
        if (hm != null)
            hm.OnDeploymentChaneged -= UpdateInfo;
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
        //세부 정보 띄우기
        //GameManager.uiManager.OpenWindow();
    }
}
