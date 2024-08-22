using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class UIDevelop : MonoBehaviour
{
    public Button buttonVillage;
    public Button buttonHuntZone;
    public TextMeshProUGUI textHuntZone;

    //public TextMeshProUGUI currentStageText;

    public GameObject onVillage;
    //public GameObject onHuntZone;
    //public TextMeshProUGUI textBossButton;
    public TMP_InputField inputStageNum;

    private bool isDevelopTextOn = true;

    public TextMeshProUGUI playerLevel;
    public TextMeshProUGUI gold;
    private List<Sprite> loadedSprite = new();
    public List<Image> itemSprites = new();
    public List<TextMeshProUGUI> itemTexts = new();
    public Slider expBar;

    public TextMeshProUGUI textVersion;

    public Button constructButton;
    public Button changePlacement;
    public Button rotateBuilding;
    public Button destroyBuilding;

    public GameObject constructComplete;

    public GameObject levelUpPopUp;

    private void Awake()
    {
        GameManager.Subscribe(EVENT_TYPE.INIT, OnGameInit);
        GameManager.Subscribe(EVENT_TYPE.GAME_READY, OnGameReady);
    }

    private void OnGameInit()
    {
        var path = "Assets/Scenes/Design/Icon/";
        var itemDatas = DataTableManager.itemTable.GetDatas();
        for(int i = 0; i < itemDatas.Count; ++i)
        {
            var newPath = $"{path}{itemDatas[i].CurrencyAssetFileName}.png";
            Addressables.LoadAssetAsync<Sprite>(newPath).Completed += OnLoadDone;
        }
    }

    private void OnGameReady()
    {
        SetExpBar();
        var itemDatas = DataTableManager.itemTable.GetDatas();
        for (int i = 0; i < itemSprites.Count; ++i)
        {
            GameManager.itemManager.ownItemList.TryGetValue(itemDatas[i].TableID, out int value);
            itemTexts[i].text = value.ToString();
        }
    }

    public void SetExpBar()
    {
        var pm = GameManager.playerManager;
        expBar.value = (float)pm.Exp / (float)DataTableManager.playerTable.GetData(pm.level).Exp;
    }

    private void OnLoadDone(AsyncOperationHandle<Sprite> obj)
    {
        loadedSprite.Add(obj.Result);
        var index = loadedSprite.IndexOf(obj.Result);
        itemSprites[index].sprite = obj.Result;
    }

    public void OnButtonVillage()
    {
        //onVillage.SetActive(true);

        //Camera.main.transform.position = Vector3.zero + Vector3.forward * -10f;
        constructButton.gameObject.SetActive(true);
        GameManager.cameraManager.SetLocation(LOCATION.VILLAGE);
    }

    public void OnButtonHuntZone()
    {
        //onVillage.SetActive(false);
        constructButton.gameObject.SetActive(false);
        var constructMode = GameManager.uiManager.windows[WINDOW_NAME.CONSTRUCT_MODE] as UIConstructMode;
        if(GameManager.villageManager.constructMode.isConstructMode)
        {
            constructMode.FinishConstructMode();
            GameManager.Publish(EVENT_TYPE.CONSTRUCT);
        }
            

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
        //TODO 사용하지 않는 버튼. 제거 필요
        //GameManager.huntZoneManager.HuntZones[GameManager.cameraManager.HuntZoneNum].SpawnUnit();
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

    public void SetPlayerLevel()
    {
        playerLevel.text = $"{GameManager.playerManager.level.ToString()}";
    }

    public void SetItem(int id, int value)
    {
        int adjust = 8000001;
        itemTexts[id - adjust].text = value.ToString();
    }

    public void GoldCheat()
    {
        GameManager.itemManager.Gold += 1000;
    }

    public void SetConstructMode()
    {
        //활성화되었던 다른 UI 닫기
        foreach(var window in GameManager.uiManager.windows.Values)
        {
            if(window.isOpened)
            {
                window.Close();
            }
        }

        GameManager.Publish(EVENT_TYPE.CONSTRUCT);
        GameManager.uiManager.windows[WINDOW_NAME.CONSTRUCT_MODE].Open();
    }

    public void TouchBuildingInConstructMode()
    {
        changePlacement.gameObject.SetActive(true);
        rotateBuilding.gameObject.SetActive(true);
        destroyBuilding.gameObject.SetActive(true);
        SetButtonColor();
    }

    public void SetButtonColor()
    {
        Color falseColor = new Color(255f / 255f, 128f / 255f, 128f / 255f);
        Color trueColor = new Color(200f / 255f, 231f / 255f, 167f / 255f);

        if (!GameManager.uiManager.currentNormalBuidling.CanReplace)
        {
            changePlacement.interactable = false;
            ColorBlock colorBlock = changePlacement.colors;
            colorBlock.normalColor = falseColor;
            colorBlock.selectedColor = falseColor;
            changePlacement.colors = colorBlock;
        }
        else
        {
            changePlacement.interactable = true;
            ColorBlock colorBlock = changePlacement.colors;
            colorBlock.normalColor = trueColor;
            colorBlock.selectedColor = trueColor;
            changePlacement.colors = colorBlock;
        }

        if (!GameManager.uiManager.currentNormalBuidling.CanReverse)
        {
            rotateBuilding.interactable = false;
            ColorBlock colorBlock = rotateBuilding.colors;
            colorBlock.normalColor = falseColor;
            colorBlock.selectedColor = falseColor;
            rotateBuilding.colors = colorBlock;
        }
        else
        {
            rotateBuilding.interactable = true;
            ColorBlock colorBlock = rotateBuilding.colors;
            colorBlock.normalColor = trueColor;
            colorBlock.selectedColor = trueColor;
            rotateBuilding.colors = colorBlock;
        }

        if (!GameManager.uiManager.currentNormalBuidling.CanDestroy)
        {
            destroyBuilding.interactable = false;
            ColorBlock colorBlock = destroyBuilding.colors;
            colorBlock.normalColor = falseColor;
            colorBlock.selectedColor = falseColor;
            destroyBuilding.colors = colorBlock;
        }
        else
        {
            destroyBuilding.interactable = true;
            ColorBlock colorBlock = destroyBuilding.colors;
            colorBlock.normalColor = trueColor;
            colorBlock.selectedColor = trueColor;
            destroyBuilding.colors = colorBlock;
        }
    }

    public void ConstructButtonsOff()
    {
        changePlacement.gameObject.SetActive(false);
        rotateBuilding.gameObject.SetActive(false);
        destroyBuilding.gameObject.SetActive(false);
    }

    public void OnButtonOption()
    {
        GameManager.uiManager.windows[WINDOW_NAME.OPTION].Open();
    }

    public void OnButtonQuit()
    {
        var constructMode = GameManager.uiManager.windows[WINDOW_NAME.CONSTRUCT_MODE] as UIConstructMode;
        if (GameManager.villageManager.constructMode.isConstructMode)
        {
            constructMode.FinishConstructMode();
            GameManager.Publish(EVENT_TYPE.CONSTRUCT);
        }

        GameManager.GameQuit();
    }

    public void SetHuntzoneStage()
    {
        //if (GameManager.huntZoneManager.HuntZones.ContainsKey(GameManager.cameraManager.HuntZoneNum))
        //    currentStageText.text = $"STAGE {GameManager.huntZoneManager.HuntZones[GameManager.cameraManager.HuntZoneNum].Stage.ToString()}";
    }

    public void LevelUp()
    {
        levelUpPopUp.SetActive(true);
        levelUpPopUp.GetComponent<LevelUpPopUp>().SetPopUp();
        SetExpBar();
        SetPlayerLevel();
    }

    public void OnButtonPlacement()
    {
        GameManager.uiManager.windows[WINDOW_NAME.PLACEMENT].Open();
    }

    private void Start()
    {
        SetPlayerLevel();
        textHuntZone.text = $"HuntZone {1}";
        inputStageNum.text = 1.ToString();
        //onHuntZone.SetActive(false);
        textVersion.text = Application.version;
    }

    private void Update()
    {
        //onHuntZone.SetActive(GameManager.cameraManager.LookLocation == LOCATION.HUNTZONE);

        var huntNum = GameManager.cameraManager.HuntZoneNum;
        if (!GameManager.huntZoneManager.HuntZones.ContainsKey(GameManager.cameraManager.HuntZoneNum))
            huntNum = 1;
        textHuntZone.text = $"HuntZone {huntNum}";


        //if (GameManager.huntZoneManager.HuntZones.ContainsKey(GameManager.cameraManager.HuntZoneNum)
        //    && !GameManager.huntZoneManager.HuntZones[GameManager.cameraManager.HuntZoneNum].CanSpawnBoss)
        //{
        //    textBossButton.text = $"{GameManager.huntZoneManager.HuntZones[GameManager.cameraManager.HuntZoneNum].BossTimer:00} | {GameManager.huntZoneManager.HuntZones[GameManager.cameraManager.HuntZoneNum].RetryTimer:00}";
        //}
        //else
        //{
        //    textBossButton.text = "보스 소환";
        //}

        SetHuntzoneStage();
    }
}
