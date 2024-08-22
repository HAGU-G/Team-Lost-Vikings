using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICameraMove : MonoBehaviour
{
    public Transform buttonTransform;
    public Button villageButton;
    public GameObject buttonPrefab;
    public Button close;

    private void Awake()
    {
        GameManager.Subscribe(EVENT_TYPE.START, OnGameStart);
    }

    private void OnGameStart()
    {
        villageButton.onClick.AddListener(() =>
            {
                GameManager.cameraManager.SetLocation(LOCATION.VILLAGE);
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
                GameManager.cameraManager.SetLocation(LOCATION.HUNTZONE, huntzoneNum + 1);
            });
        }

        close.onClick.AddListener(() =>
        {
            var animator = GetComponent<Animator>();
            animator.SetBool("move", false);
        });
    }
}
