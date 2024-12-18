﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillInformation : MonoBehaviour
{
    public TextMeshProUGUI skillName;
    public Image skillIcon;
    public TextMeshProUGUI skillDesc;
    public TextMeshProUGUI skillCool;
    public TextMeshProUGUI skillDetailDesc;
    public Button exit;

    private void Awake()
    {
        exit.onClick.AddListener(OnButtonExit);
    }

    private void OnButtonExit()
    {
        GameManager.PlayButtonSFX();
        gameObject.SetActive(false);
    }
}
