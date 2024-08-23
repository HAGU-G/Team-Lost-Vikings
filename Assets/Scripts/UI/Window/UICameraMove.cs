using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UIWindowMessage;

public class UICameraMove : MonoBehaviour
{
    public Transform buttonTransform;
    public Button villageButton;
    public GameObject buttonPrefab;
    public Button close;
    public GameObject constructButton;

    private void Awake()
    {
        GameManager.Subscribe(EVENT_TYPE.START, OnGameStart);
        constructButton.SetActive(true);
    }

    private void OnGameStart()
    {
        villageButton.onClick.AddListener(() =>
            {
                GameManager.cameraManager.SetLocation(LOCATION.VILLAGE);
                constructButton.SetActive(true);
            });

        var huntzones = GameManager.huntZoneManager.HuntZones;
        for (int i = 0; i < huntzones.Count; ++i)
        {
            int huntzoneNum = i;
            var location = Instantiate(buttonPrefab, buttonTransform);
            var button = location.GetComponent<Button>();
            var text = location.GetComponentInChildren<TextMeshProUGUI>();
            text.text = $"{huntzoneNum + 1}번 사냥터";
            button.onClick.AddListener(() =>
            {
                var requireLv = GameManager.huntZoneManager.HuntZones[huntzoneNum + 1].GetCurrentData().RequirePlayerLv;
                if (GameManager.playerManager.level < requireLv)
                {
                    var message = GameManager.uiManager.windows[WINDOW_NAME.MESSAGE_POPUP] as UIWindowMessage;
                    message.ShowMessage(
                        $"유저 레벨 {requireLv}에 들어갈 수 있습니다.",
                        true,
                        1.2f,
                        openAnimation: UIWindowMessage.OPEN_ANIMATION.FADEINOUT,
                        closeType: CLOSE_TYPE.TOUCH);
                    return;
                }

                GameManager.cameraManager.SetLocation(LOCATION.HUNTZONE, huntzoneNum + 1);
                var constructMode = GameManager.uiManager.windows[WINDOW_NAME.CONSTRUCT_MODE] as UIConstructMode;
                if (GameManager.villageManager.constructMode.isConstructMode)
                {
                    constructMode.FinishConstructMode();
                    GameManager.Publish(EVENT_TYPE.CONSTRUCT);
                }
                constructButton.SetActive(false);
            });

            
        }

        close.onClick.AddListener(() =>
        {
            var animator = GetComponent<Animator>();
            animator.SetBool("move", false);
        });
    }
}
