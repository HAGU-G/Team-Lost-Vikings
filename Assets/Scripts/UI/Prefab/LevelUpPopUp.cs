using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpPopUp : MonoBehaviour
{
    public TextMeshProUGUI prevLv;
    public TextMeshProUGUI currLv;

    public Button close;

    private void Start()
    {
        //close.onClick.AddListener(OnButtonClose);
    }

    private void OnEnable()
    {
    }

    public void SetPopUp()
    {
        prevLv.text = $"Lv.{GameManager.playerManager.prevLevel}";
        currLv.text = $"Lv.{GameManager.playerManager.level}";

        PlayLevelUpSfx();
        PlayParticle();
    }

    private void PlayLevelUpSfx()
    {
        //SoundManager.PlaySFX();
    }

    private void PlayParticle()
    {
        GameManager.effectManager.GetEffect("LevelUp");
    }

    private void Update()
    {
        if (GameManager.inputManager.Tap)
            gameObject.SetActive(false);
            
    }
}
