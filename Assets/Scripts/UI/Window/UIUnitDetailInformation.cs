using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

public class UIUnitDetailInformation : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.UNIT_DETAIL_INFORMATION;

    public Button placement;
    public Button exit;

    public TextMeshProUGUI characterName;
    public Image characterIcon;
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
        unit = GameManager.uiManager.currentUnitStats;
        SetInfo();
    }

    public void SetInfo()
    {
        characterName.text = unit.Name;
        //characterIcon.sprite = 
        //gradeIcon.sprite = 
        characterJob.text = unit.Job.ToString();

        //skill1_Icon.sprite = 
        skill1_Name.text = DataTableManager.skillTable.GetData(unit.SkillId1).Name;
        //skill1_Desc.text = DataTableManager.skillTable.GetData(unit.SkillId1).; //스킬 설명 컬럼 추가 필요
        //skill2_Icon.sprite = 
        skill2_Name.text = DataTableManager.skillTable.GetData(unit.SkillId2).Name;
        //skill2_Desc.text = DataTableManager.skillTable.GetData(unit.SkillId2).; //스킬 설명 컬럼 추가 필요

        //attributeIcon.sprite = ;
        attributeText.text = unit.BasicAttackType.ToString();

        //hpIcon.sprite = ;
        hpBar.value = (float)unit.HP.Current / (float)unit.HP.max;
        //staminaIcon.sprite = ;
        staminaBar.value = (float)unit.Stamina.Current / (float)unit.Stamina.max;
        //stressIcon.sprite = ;
        stressBar.value = (float)unit.Stress.Current / (float)unit.Stress.max;

        totalCombatValue.text = unit.CombatPoint.ToString();
        combatDetail.onClick.AddListener(
            () => { });
        //strIcon.sprite = ;
        strValue.text = unit.StrWeight.Current.ToString();
        //magIcon.sprite = ;
        magValue.text = unit.WizWeight.Current.ToString();
        //agiIcon.sprite = ;
        agiValue.text = unit.AgiWeight.Current.ToString();

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
        GameManager.uiManager.windows[(int)WINDOW_NAME.CHARACTER_LOCATE].Open();
    }

    public void OnButtonExit()
    {
        Close();
    }
}