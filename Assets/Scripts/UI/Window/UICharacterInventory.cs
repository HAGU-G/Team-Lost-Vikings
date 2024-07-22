using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif


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
                    if (character.Value.objectTransform == null)
                    {
                        character.Value.SetHuntZone(1);
                    }
                    else
                    {
                        GameManager.uiManager.OnShowCharacter(character.Value.InstanceID);
                    }
                });
        }
    }
}
