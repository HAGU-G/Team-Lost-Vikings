using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class UICharacterWaiting : UIWindow
{
    public GameObject characterbuttonPrefab;
    public ScrollRect scrollRect;

    public override WINDOW_NAME WindowName => WINDOW_NAME.CHARACTER_INVENTORY;

    protected override void Awake()
    {
        base.Awake();
        GameManager.uiManager.chracterWaiting = this;
        LoadCharacterButtons(GameManager.unitManager.Waitings);
    }

    public void LoadCharacterButtons(Dictionary<int, UnitStats> units)
    {
        for (int i = scrollRect.content.childCount - 1; i >= 0; i--)
        {
            Destroy(scrollRect.content.GetChild(i).gameObject);
        }

        foreach (var character in units)
        {
            var button = Instantiate(characterbuttonPrefab, scrollRect.content).GetComponent<Button>();
            button.GetComponentInChildren<TextMeshProUGUI>().text = character.Value.InstanceID.ToString();

            button.GetComponentInChildren<RawImage>().uvRect
                = GameManager.uiManager.unitRenderTexture.LoadRenderTexture(character.Value.AssetFileName);

            button.onClick.AddListener(
                () =>
                {
                    GameManager.uiManager.OnPickUpCharacter(character.Value.InstanceID);
                    Destroy(button.gameObject);
                });
        }
    }
}