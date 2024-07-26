﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDevelop : MonoBehaviour
{
    public Button buttonVillage;
    public Button buttonHuntZone;
    public TextMeshProUGUI textHuntZone;

    public TextMeshProUGUI currentStageText;

    public GameObject onVillage;
    public GameObject onHuntZone;
    public TextMeshProUGUI textBossButton;
    public TMP_InputField inputStageNum;

    private bool isDevelopTextOn = true;

    public TextMeshProUGUI villageLevel;
    public TextMeshProUGUI gold;

    public TextMeshProUGUI textVersion;

    public void OnButtonVillage()
    {
        //onVillage.SetActive(true);

        //Camera.main.transform.position = Vector3.zero + Vector3.forward * -10f;
        GameManager.cameraManager.SetLocation(LOCATION.VILLAGE);
    }

    public void OnButtonHuntZone()
    {
        //onVillage.SetActive(false);

        if (GameManager.cameraManager.LookLocation != LOCATION.HUNTZONE)
        {
            if (!GameManager.huntZoneManager.HuntZones.ContainsKey(GameManager.cameraManager.HuntZoneNum))
                GameManager.cameraManager.SetLocation(LOCATION.HUNTZONE, 1);
            else
                GameManager.cameraManager.SetLocation(LOCATION.HUNTZONE, GameManager.cameraManager.HuntZoneNum);

        }
        else
        {
            var currentHuntZone = GameManager.cameraManager.HuntZoneNum;
            currentHuntZone++;

            if (currentHuntZone > GameManager.huntZoneManager.HuntZones.Count)
            {
                currentHuntZone = 1;
            }
            GameManager.cameraManager.SetLocation(LOCATION.HUNTZONE, currentHuntZone);
        }

        textHuntZone.text = $"HuntZone {GameManager.cameraManager.HuntZoneNum}";
        //Camera.main.transform.position = GameManager.huntZoneManager.HuntZones[currentHuntZone].transform.position + Vector3.forward * -10f;
    }

    public void OnButtonDevelopText()
    {
        GameManager.villageManager.SetDevelopText(isDevelopTextOn);
        GameManager.huntZoneManager.SetDevelopText(isDevelopTextOn);
        isDevelopTextOn = !isDevelopTextOn;
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
        //GameManager.villageManager.village.UnitSpawn();
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
        GameManager.huntZoneManager.HuntZones[GameManager.cameraManager.HuntZoneNum].SpawnMonster(1);
    }
    public void OnButtonSpawnUnitOnHunt()
    {
        GameManager.huntZoneManager.HuntZones[GameManager.cameraManager.HuntZoneNum].SpawnUnit();
    }
    public void OnButtonKillLastSpawnedMonster()
    {
        GameManager.huntZoneManager.HuntZones[GameManager.cameraManager.HuntZoneNum].KillLastSpawnedMonster();

    }
    public void OnButtonUpdateRegenPoint()
    {
        GameManager.huntZoneManager.HuntZones[GameManager.cameraManager.HuntZoneNum].UpdateRegenPoints();
    }

    public void OnButtonChangeStage()
    {
        GameManager.huntZoneManager.HuntZones[GameManager.cameraManager.HuntZoneNum].SetStage(int.Parse(inputStageNum.text));
    }
    public void OnButtonSpawnBoss()
    {
        if (!GameManager.huntZoneManager.HuntZones[GameManager.cameraManager.HuntZoneNum].CanSpawnBoss)
            return;

        GameManager.huntZoneManager.HuntZones[GameManager.cameraManager.HuntZoneNum].ResetHuntZone(false);
        GameManager.huntZoneManager.HuntZones[GameManager.cameraManager.HuntZoneNum].StartBossBattle();
    }

    public void OnButtonTutorialPopUp()
    {
        GameManager.uiManager.windows[(int)WINDOW_NAME.TUTORIAL_POPUP].Open();
    }

    public void OnButtonGachaUI()
    {
        GameManager.uiManager.windows[WINDOW_NAME.GACHA_UI].Open();
    }

    public void OnButtonUnit()
    {
        GameManager.uiManager.windows[WINDOW_NAME.UNITS_INFORMATION].Open();
    }

    public void OnButtonUnitStash()
    {
        GameManager.uiManager.windows[WINDOW_NAME.CHARACTER_STASH].Open();
    }

    public void OnButtonPrepare()
    {
        GameManager.uiManager.windows[WINDOW_NAME.WAIT_FOR_CBT].Open();
    }

    public void SetVillageLevel()
    {
        villageLevel.text = $"마을 회관 \nLv : {GameManager.villageManager.PlayerLevel.ToString()}";
        gold.text = $"{GameManager.itemManager.Gold}";
    }

    public void SetGold(int gold)
    {
        this.gold.text = gold.ToString();
    }

    public void GoldCheat()
    {

        GameManager.itemManager.Gold += 10000;
    }

    public void OnButtonQuit()
    {
        GameManager.GameQuit();
    }

    public void SetHuntzoneStage()
    {
        if (GameManager.huntZoneManager.HuntZones.ContainsKey(GameManager.cameraManager.HuntZoneNum))
            currentStageText.text = $"STAGE {GameManager.huntZoneManager.HuntZones[GameManager.cameraManager.HuntZoneNum].Stage.ToString()}";
    }

    private void Start()
    {
        SetVillageLevel();
        textHuntZone.text = $"HuntZone {1}";
        inputStageNum.text = 1.ToString();
        onHuntZone.SetActive(false);
        textVersion.text = Application.version;
    }

    private void Update()
    {
        onHuntZone.SetActive(GameManager.cameraManager.LookLocation == LOCATION.HUNTZONE);

        var huntNum = GameManager.cameraManager.HuntZoneNum;
        if (!GameManager.huntZoneManager.HuntZones.ContainsKey(GameManager.cameraManager.HuntZoneNum))
            huntNum = 1;
        textHuntZone.text = $"HuntZone {huntNum}";


        if (GameManager.huntZoneManager.HuntZones.ContainsKey(GameManager.cameraManager.HuntZoneNum)
            && !GameManager.huntZoneManager.HuntZones[GameManager.cameraManager.HuntZoneNum].CanSpawnBoss)
        {
            textBossButton.text = $"{GameManager.huntZoneManager.HuntZones[GameManager.cameraManager.HuntZoneNum].BossTimer:00} | {GameManager.huntZoneManager.HuntZones[GameManager.cameraManager.HuntZoneNum].RetryTimer:00}";
        }
        else
        {
            textBossButton.text = "보스 소환";
        }

        SetHuntzoneStage();
    }
}
