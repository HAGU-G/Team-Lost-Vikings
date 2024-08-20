using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpPopUp : MonoBehaviour
{
    public TextMeshProUGUI prevLv;
    public TextMeshProUGUI currLv;

    public Button close;

    private EffectObject effect = null;

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
        effect = GameManager.effectManager.GetEffect("LevelUp",SORT_LAYER.OverUI);
        effect.transform.localScale = new Vector3(5f,5f,5f);
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
            
            
    }
}
