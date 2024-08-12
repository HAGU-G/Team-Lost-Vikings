using TMPro;
using UnityEngine.UI;

public class UIUnitDetailInformation : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.UNIT_DETAIL_INFORMATION;

    public Button placement;
    public Button exit;

    public TextMeshProUGUI characterName;
    public RawImage characterIcon;
    public Image gradeIcon;
    public TextMeshProUGUI characterJob;

    public Image skill1_Icon;
    public TextMeshProUGUI skill1_Name;
    public TextMeshProUGUI skill1_Desc;
    public Image skill2_Icon;
    public TextMeshProUGUI skill2_Name;
    public TextMeshProUGUI skill2_Desc;

    public Image attributeIcon;
    public TextMeshProUGUI attributeText;
    public Image hpIcon;
    public Slider hpBar;
    public Image staminaIcon;
    public Slider staminaBar;
    public Image stressIcon;
    public Slider stressBar;

    public TextMeshProUGUI totalCombatValue;
    public Button combatDetail;
    public Image strIcon;
    public TextMeshProUGUI strValue;
    public Image magIcon;
    public TextMeshProUGUI magValue;
    public Image agiIcon;
    public TextMeshProUGUI agiValue;

    public Image attackSpeedIcon;
    public TextMeshProUGUI attackSpeedValue;
    public Image moveSpeedIcon;
    public TextMeshProUGUI moveSpeedValue;
    public Image critIcon;
    public TextMeshProUGUI critValue;
    public Image critDamageIcon;
    public TextMeshProUGUI critDamageValue;

    public UnitStats unit;

    private void OnEnable()
    {
        if (!IsReady)
            return;

        unit = GameManager.uiManager.currentUnitStats;
        UnityEngine.Debug.Log(GameManager.uiManager.unitRenderTexture);
        SetInfo();
    }

    private void Update()
    {
        SetParameterBar();
    }

    public void SetParameterBar()
    {
        hpBar.value = (float)unit.HP.Current / (float)unit.HP.max;
        staminaBar.value = (float)unit.Stamina.Current / (float)unit.Stamina.max;
        stressBar.value = (float)unit.Stress.Current / (float)unit.Stress.max;
    }

    public void SetInfo()
    {
        characterName.text = unit.Data.Name;
        characterIcon.uvRect
                = GameManager.uiManager.unitRenderTexture.LoadRenderTexture(unit.Data.UnitAssetFileName);
        gradeIcon.sprite = GameManager.uiManager.gradeIcons[(int)unit.UnitGrade];
        characterJob.text = unit.Data.Job.ToString();

        //스킬 정보 임시 수정
        //TODO 스킬이 없을 경우 처리 필요
        if (unit.Skills.Count >= 1)
        {
            //skill1_Icon.sprite = 
            skill1_Name.text = unit.Skills[0].Data.SkillName;
            skill1_Desc.text = unit.Skills[0].Data.SkillDesc;
        }
        if (unit.Skills.Count >= 2)
        {
            //skill2_Icon.sprite = 
            skill2_Name.text = unit.Skills[1].Data.SkillName;
            skill1_Desc.text = unit.Skills[1].Data.SkillDesc;
        }
        //attributeIcon.sprite = ;
        attributeText.text = unit.Data.BasicAttackType.ToString();

        //hpIcon.sprite = ;
        hpBar.interactable = false;
        hpBar.value = (float)unit.HP.Current / (float)unit.HP.max;
        //staminaIcon.sprite = ;
        staminaBar.interactable = false;
        staminaBar.value = (float)unit.Stamina.Current / (float)unit.Stamina.max;
        //stressIcon.sprite = ;
        stressBar.interactable = false;
        stressBar.value = (float)unit.Stress.Current / (float)unit.Stress.max;

        totalCombatValue.text = unit.CombatPoint.ToString();
        combatDetail.onClick.AddListener(
            () => { });
        //strIcon.sprite = ;
        strValue.text = unit.BaseStr.Current.ToString();
        //magIcon.sprite = ;
        magValue.text = unit.BaseWiz.Current.ToString();
        //agiIcon.sprite = ;
        agiValue.text = unit.BaseAgi.Current.ToString();

        //attackSpeedIcon.sprite = ;
        attackSpeedValue.text = unit.AttackSpeed.Current.ToString();
        //moveSpeedIcon.sprite = ;
        moveSpeedValue.text = unit.MoveSpeed.Current.ToString();
        //critIcon.sprite = ;
        critValue.text = unit.CritChance.Current.ToString();
        //critDamageIcon.sprite = ;
        critDamageValue.text = unit.CritWeight.Current.ToString();
    }

    public void OnButtonPlacement()
    {
        GameManager.uiManager.windows[WINDOW_NAME.CHARACTER_LOCATE].Open();
    }

    public void OnButtonExit()
    {
        Close();
    }
}