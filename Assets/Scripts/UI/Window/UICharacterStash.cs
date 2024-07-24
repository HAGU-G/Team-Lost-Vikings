using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterStash : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.CHARACTER_STASH;

    public Button exit;
    public TextMeshProUGUI gachaCycleText;

    public ScrollRect scrollRect;
    public GameObject characterbuttonPrefab;

    public void OnButtonExit()
    {
        Close();

    }

    private void Update()
    {
        SetGachaText(GameManager.unitManager.TimeToAutoGacha);
    }

    public void SetGachaText(float time)
    {
        gachaCycleText.text = $"주기적 가챠 시간 : {time:0.0}초 남음";
    }

    private void OnEnable()
    {
        if (!IsReady)
            return;

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
            var button = Instantiate(characterbuttonPrefab, scrollRect.content);
            var unit = button.GetComponent<StashCharacter>();
            unit.characterName.text = $"{character.Value.Name} / {character.Value.UnitGrade}";
            //unit.rarity.sprite = ;
            unit.recruit.onClick.AddListener(
            () =>
            {
                    GameManager.unitManager.PickUpCharacter(character.Value.InstanceID);
                    Destroy(button.gameObject);
                }
                );
        }
    }
}
