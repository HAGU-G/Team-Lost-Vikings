using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class UICharacterInventory : UIWindow
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

            Addressables.InstantiateAsync(character.Value.AssetFileName, button.transform)
                .Completed += (handle) =>
                {
                    handle.Result.gameObject.transform.localScale = Vector3.one * 40f;
                    handle.Result.gameObject.transform.localPosition += Vector3.down * 20f;
                };

            button.onClick.AddListener(
                () => GameManager.uiManager.OnShowCharacter(character.Value.InstanceID)
                );
        }
    }
}