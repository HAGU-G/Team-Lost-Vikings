﻿using UnityEngine;
using UnityEngine.UI;
public class UICheat : MonoBehaviour
{
    public Button buttonGold;
    public Button buttonAll;
    public Button buttonLevel;

    private void Awake()
    {
        buttonGold.onClick.AddListener(() =>
        {
            GameManager.PlayButtonSFX();
            GameManager.itemManager?.CheatGold(5000);
        });
        buttonAll.onClick.AddListener(() =>
        {
            GameManager.PlayButtonSFX();
            GameManager.itemManager?.CheatAllItem(5000);
        });
        buttonLevel.onClick.AddListener(() =>
        {
            GameManager.PlayButtonSFX();
            GameManager.itemManager?.CheatLevel(5000);
        });
    }
}