using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDevelop : MonoBehaviour
{
    public Button buttonVillage;
    public Button buttonHuntZone;
    public TextMeshProUGUI textHuntZone;

    public GameObject onVillage;
    public GameObject onHuntZone;
    public TextMeshProUGUI textBossButton;
    public TMP_InputField inputStageNum;

    private int currentHuntZone = 1;
    private bool isShowVillage = true;

    public void OnButtonVillage()
    {
        onVillage.SetActive(true);
        onHuntZone.SetActive(false);

        isShowVillage = true;

        Camera.main.transform.position = Vector3.zero + Vector3.forward * -10f;
    }

    public void OnButtonHuntZone()
    {
        onVillage.SetActive(false);
        onHuntZone.SetActive(true);

        if (isShowVillage)
        {
            isShowVillage = false;
        }
        else
        {
            currentHuntZone++;
            if (currentHuntZone > GameManager.huntZoneManager.HuntZones.Count)
            {
                currentHuntZone = 1;
            }
        }

        textHuntZone.text = $"HuntZone {currentHuntZone}";
        Camera.main.transform.position = GameManager.huntZoneManager.HuntZones[currentHuntZone].transform.position + Vector3.forward * -10f;
    }

    public void OnButtonReduceHP()
    {
        GameManager.villageManager.village.ReduceHp();
    }
    public void OnButtonReduceStamina()
    {
        GameManager.villageManager.village.ReduceStamina();
    }
    public void OnButtonReduceStress()
    {
        GameManager.villageManager.village.ReduceStress();
    }
    public void OnButtonSpawnUnitOnVillage()
    {
        GameManager.villageManager.village.UnitSpawn();
    }

    public void OnButtonUpgrade()
    {
        GameManager.villageManager.village.Upgrade();
    }

    public void OnButtonCancel()
    {
        GameManager.villageManager.village.Cancel();
    }

    public void OnButtonSpawnMonster()
    {
        GameManager.huntZoneManager.HuntZones[currentHuntZone].SpawnMonster(1);
    }
    public void OnButtonSpawnUnitOnHunt()
    {
        GameManager.huntZoneManager.HuntZones[currentHuntZone].SpawnUnit();
    }
    public void OnButtonKillLastSpawnedMonster()
    {
        GameManager.huntZoneManager.HuntZones[currentHuntZone].KillLastSpawnedMonster();

    }
    public void OnButtonUpdateRegenPoint()
    {
        GameManager.huntZoneManager.HuntZones[currentHuntZone].UpdateRegenPoints();
    }

    public void OnButtonChangeStage()
    {
        GameManager.huntZoneManager.HuntZones[currentHuntZone].SetStage(int.Parse(inputStageNum.text));
        GameManager.huntZoneManager.HuntZones[currentHuntZone].ResetHuntZone(false);
    }
    public void OnButtonSpawnBoss()
    {
        if (!GameManager.huntZoneManager.HuntZones[currentHuntZone].CanSpawnBoss)
            return;

        GameManager.huntZoneManager.HuntZones[currentHuntZone].ResetHuntZone(false);
        GameManager.huntZoneManager.HuntZones[currentHuntZone].StartBossBattle();
    }

    private void Start()
    {
        textHuntZone.text = $"HuntZone {currentHuntZone}";
        inputStageNum.text = 1.ToString();
        onHuntZone.SetActive(false);
    }

    private void Update()
    {
        if (!GameManager.huntZoneManager.HuntZones[currentHuntZone].CanSpawnBoss)
        {
            textBossButton.text = $"{GameManager.huntZoneManager.HuntZones[currentHuntZone].BossTimer:00} | {GameManager.huntZoneManager.HuntZones[currentHuntZone].RetryTimer:00}";
        }
        else
        {
            textBossButton.text = "보스 소환";
        }
    }
}
