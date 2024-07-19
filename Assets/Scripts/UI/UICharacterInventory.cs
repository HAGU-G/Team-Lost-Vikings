﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterInventory : IWindowable
{
    public GameObject characterbuttonPrefab;
    public ScrollRect scrollRect;

    public override WINDOW_NAME WindowName => WINDOW_NAME.CHARACTER_INVENTORY;

    private void Awake()
    {
        GameManager.uiManager.chracterInventory = this;
        LoadCharacterButtons(GameManager.unitManager.Units);
    }

    public void LoadCharacterButtons(Dictionary<int,UnitStats> units)
    {
        foreach (var character in units)
        {
            var button = Instantiate(characterbuttonPrefab, scrollRect.content).GetComponent<Button>();
            button.GetComponentInChildren<TextMeshProUGUI>().text = character.Value.InstanceID.ToString();

            button.onClick.AddListener(
                () => GameManager.uiManager.OnShowCharacter(character.Value.InstanceID)
                );
        }
    }
}