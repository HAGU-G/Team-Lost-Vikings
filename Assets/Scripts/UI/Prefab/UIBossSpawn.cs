using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBossSpawn : MonoBehaviour
{
    public Image imageIcon;
    public Button buttonSpawn;
    public TextMeshProUGUI textButton;

    public Animator animator;

    [Header("아이콘")]
    public Sprite iconDownArrow;
    public Sprite iconBattle;
    public Sprite iconLock;

    private CameraManager cm = null;
    private HuntZoneManager hm = null;
    private UIManager um = null;

    public static readonly int paramPlay = Animator.StringToHash("Play");
    public static readonly string stringBossSpawn = "보스 소환";
    public static readonly string formatSpawnMessage = "보스 등장\n{0}";
    public static readonly string formatTimer = "{0:0}";

    private HuntZone currentHuntZone = null;

    private void Awake()
    {
        GameManager.Subscribe(EVENT_TYPE.INIT, OnGameInit);

        buttonSpawn.onClick.AddListener(SpawnBoss);
        buttonSpawn.onClick.AddListener(GameManager.PlayButtonSFX);
    }

    private void OnGameInit()
    {
        hm = GameManager.huntZoneManager;
        hm.OnHuntZoneInfoChanged += UpdateInfo;

        cm = GameManager.cameraManager;
        cm.OnLocationChanged += UpdateInfo;

        um = GameManager.uiManager;
    }

    private void OnDestroy()
    {
        if (hm != null)
            hm.OnHuntZoneInfoChanged -= UpdateInfo;
    }

    private void Update()
    {
        if (hm == null || currentHuntZone == null)
            return;

        UpdateInfo();
    }

    private void SpawnBoss()
    {
        if (currentHuntZone == null)
            return;

        if (currentHuntZone.CanSpawnBoss)
        {
            var message = um.windows[WINDOW_NAME.MESSAGE_POPUP] as UIWindowMessage;
            message.ShowMessage(
                string.Format(formatSpawnMessage, currentHuntZone.GetCurrentBoss()?.Name),
                true,
                1.3f,
                onClose: StartBossBattle,
                openAnimation: UIWindowMessage.OPEN_ANIMATION.BLACKOUT
                );
        }
    }

    private void StartBossBattle()
    {
        currentHuntZone.ResetHuntZone(false);
        currentHuntZone.StartBossBattle();
    }


    public void UpdateInfo()
    {
        if (hm == null || cm == null)
            return;

        if (!(hm.HuntZones.ContainsKey(cm.HuntZoneNum)
            && cm.LookLocation == LOCATION.HUNTZONE))
        {
            gameObject.SetActive(false);
            return;
        }

        currentHuntZone = hm.HuntZones[cm.HuntZoneNum];

        if (currentHuntZone.GetCurrentBoss() == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        animator.SetBool(paramPlay, currentHuntZone.CanSpawnBoss);
        UpdateButtonText();
        UpdateIcon();
    }

    public void UpdateIcon()
    {
        imageIcon.sprite = currentHuntZone.IsBossBattle ? iconBattle
            : (currentHuntZone.CanSpawnBoss ? iconDownArrow
            : iconLock);
    }

    public void UpdateButtonText()
    {
        textButton.text = currentHuntZone.IsBossBattle ? string.Format(formatTimer, currentHuntZone.BossTimer)
            : (currentHuntZone.CanSpawnBoss ? stringBossSpawn
            : string.Format(formatTimer, currentHuntZone.RetryTimer));
    }
}