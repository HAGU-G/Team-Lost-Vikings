using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterInventory : IWindowable
{
    public GameObject characterbuttonPrefab;
    public ScrollRect scrollRect;

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



        var test = new UnitOnVillage();

        test.Init();
        test.ResetUnit(new UnitStats());
        test.transform.position = Vector3.zero;

    }
}