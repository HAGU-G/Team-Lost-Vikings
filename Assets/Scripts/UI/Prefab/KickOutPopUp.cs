using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

public class KickOutPopUp : MonoBehaviour
{
    public Button yesButton;
    public Button noButton;

    private UnitStats unit = new();

    private void Awake()
    {
        yesButton.onClick.AddListener(OnButtonYes);
        noButton.onClick.AddListener(OnButtonNo);
        unit = GameManager.uiManager.currentUnitStats;
    }

    private void OnButtonYes()
    {
        GameManager.unitManager.DiscardCharacter(unit.InstanceID);
        var detailUI = GameManager.uiManager.windows[WINDOW_NAME.UNIT_DETAIL_INFORMATION] as UIUnitDetailInformation;
        detailUI.Close();
    }

    private void OnButtonNo()
    {
        gameObject.SetActive(false);
    }
}
