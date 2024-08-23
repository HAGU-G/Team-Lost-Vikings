using UnityEngine;
using UnityEngine.UI;

public class KickOutPopUp : MonoBehaviour
{
    public Button yesButton;
    public Button noButton;

    private UnitStats unit = new();

    private void Awake()
    {
        yesButton.onClick.AddListener(OnButtonYes);
        noButton.onClick.AddListener(OnButtonNo);
    }

    private void OnButtonYes()
    {
        GameManager.PlayButtonSFX();
        unit = GameManager.uiManager.currentUnitStats;
        Debug.Log(unit.Data.Name);
        GameManager.unitManager.DiscardCharacter(unit.InstanceID);
        var detailUI = GameManager.uiManager.windows[WINDOW_NAME.UNIT_DETAIL_INFORMATION] as UIUnitDetailInformation;
        detailUI.Close();
        var stash = GameManager.uiManager.windows[WINDOW_NAME.CHARACTER_STASH] as UICharacterStash;
        stash.LoadCharacterButtons(GameManager.unitManager.Waitings);
    }

    private void OnButtonNo()
    {
        GameManager.PlayButtonSFX();
        gameObject.SetActive(false);
    }
}
