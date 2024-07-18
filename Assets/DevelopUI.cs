using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DevelopUI : MonoBehaviour
{
    public Button buttonVillage;
    public Button buttonHuntZone;
    public TextMeshProUGUI textHuntZone;

    private int currentHuntZone = 0;
    private bool isShowVillage = true;

    public void OnButtonVillage()
    {
        isShowVillage = true;
        Camera.main.transform.position = Vector3.zero + Vector3.forward * -10f;

        for (int i = 0; i < GameManager.huntZoneManager.HuntZones.Count; i++)
        {
            GameManager.huntZoneManager.HuntZones[i + 1].isShowing = false;
        }
        GameManager.villageManager.isShowing = true;
    }

    public void OnButtonHuntZone()
    {
        GameManager.villageManager.isShowing = false;
        if (isShowVillage)
        {
            isShowVillage = false;
        }
        else
        {
            currentHuntZone++;
            if (currentHuntZone >= GameManager.huntZoneManager.HuntZones.Count)
            {
                currentHuntZone = 0;
            }
            textHuntZone.text = $"HuntZone {currentHuntZone + 1}";
        }

        for (int i = 0; i < GameManager.huntZoneManager.HuntZones.Count; i++)
        {
            if (i == currentHuntZone)
                GameManager.huntZoneManager.HuntZones[i + 1].isShowing = true;
            else
                GameManager.huntZoneManager.HuntZones[i + 1].isShowing = false;
        }
        Camera.main.transform.position = GameManager.huntZoneManager.HuntZones[currentHuntZone + 1].transform.position + Vector3.forward * -10f;
    }
}
