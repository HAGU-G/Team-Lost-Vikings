using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpPopUp : MonoBehaviour
{
    public TextMeshProUGUI prevLv;
    public TextMeshProUGUI currLv;

    public Button close;

    private EffectObject effect = null;

    private float closeTime = 2.0f;
    private float timer = 0f;

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
        SoundManager.PlaySFX("LevelUP");
    }

    private void PlayParticle()
    {
        effect = GameManager.effectManager.GetEffect("PopUp_effect",SORT_LAYER.OverUI);
        effect.transform.position = transform.position;
    }

    private void Update()
    {
        if (GameManager.inputManager.Tap)
        {
            gameObject.SetActive(false);

            if (effect.isActiveAndEnabled)
                effect.Stop();
        }

        timer += Time.deltaTime;
        if(timer >= closeTime)
        {
            timer = 0f;

            gameObject.SetActive(false);

            if (effect.isActiveAndEnabled)
                effect.Stop();
        }
    }
}
