using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatDetailPopUp : MonoBehaviour
{
    public TextMeshProUGUI totalCombat;
    public TextMeshProUGUI jobCombatBonus;
    public TextMeshProUGUI strCombat;
    public TextMeshProUGUI currStr;
    public TextMeshProUGUI strApplyAmount;
    public TextMeshProUGUI magCombat;
    public TextMeshProUGUI currMag;
    public TextMeshProUGUI magApplyAmount;
    public TextMeshProUGUI agiCombat;
    public TextMeshProUGUI currAgi;
    public TextMeshProUGUI agiApplyAmount;

    public Button exit;

    private void Awake()
    {
        exit.onClick.AddListener(OnButtonExit);
    }

    private void OnButtonExit()
    {
        gameObject.SetActive(false);
    }
}
