using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class UIUnitDetailInformation : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.UNIT_DETAIL_INFORMATION;

    public Button exit;
    public Button placement;
    public Button kickOut;

    public TextMeshProUGUI characterName;
    public RawImage characterIcon;
    public Image gradeIcon;
    public TextMeshProUGUI characterJob;

    public Button skill1_Icon;
    public TextMeshProUGUI skill1_Name;
    public TextMeshProUGUI skill1_Desc;
    public Button skill2_Icon;
    public TextMeshProUGUI skill2_Name;
    public TextMeshProUGUI skill2_Desc;

    public Image attributeIcon;
    public Image hpIcon;
    public Image staminaIcon;
    public Image stressIcon;
    public Image strIcon;
    public Image magIcon;
    public Image agiIcon;
    public Image attackSpeedIcon;
    public Image moveSpeedIcon;
    public Image critIcon;
    public Image critDamageIcon;

    public TextMeshProUGUI attributeText;

    public Slider hpBar;
    public TextMeshProUGUI hpText;
    public Slider staminaBar;
    public TextMeshProUGUI staminaText;
    public Slider stressBar;
    public TextMeshProUGUI stressText;

    public Button detailButton;

    public TextMeshProUGUI totalCombatValue;
    public Button combatDetail;

    public TextMeshProUGUI strValue;
    public TextMeshProUGUI magValue;
    public TextMeshProUGUI agiValue;

    public TextMeshProUGUI attackSpeedValue;
    public TextMeshProUGUI moveSpeedValue;
    public TextMeshProUGUI critValue;
    public TextMeshProUGUI critDamageValue;

    public UnitStats unit;

    private string path = "Assets/0820Pick/Icon/";
    private Dictionary<int, Sprite> skillIcons = new();

    public GameObject skillPopUp;
    public GameObject kickOutPopUp;
    public GameObject combatPopUp;

    private void OnEnable()
    {
        if (!IsReady)
            return;
        unit = GameManager.uiManager.currentUnitStats;
        SetInfo();
        TurnOffPopUps();
    }

    protected override void OnGameStart()
    {
        base.OnGameStart();

        var datas = DataTableManager.skillTable.GetDatas();
        for (int i = 0; i < datas.Count; ++i)
        {
            var skillName = datas[i].SkillIconName;
            var newpath = $"{path}{skillName}.png";
            var id = datas[i].SkillId;

            if (Addressables.LoadResourceLocationsAsync(skillName).WaitForCompletion().Count <= 0)
            {
                if (!(skillName == string.Empty || skillName == "0"))
                {
                    Debug.LogWarning($"{skillName} 스킬 이름이 존재하지 않습니다.");
                }
                continue;
            }
            Addressables.LoadAssetAsync<Sprite>(skillName).Completed += (obj) => OnLoadDone(obj, id);
        }

        placement.onClick.AddListener(OnButtonPlacement);
        exit.onClick.AddListener(OnButtonExit);
        kickOut.onClick.AddListener(OnButtonKickOut);
    }

    private void OnLoadDone(AsyncOperationHandle<Sprite> obj, int id)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            skillIcons.Add(id, obj.Result);
        }
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

        hpText.text = $"{unit.HP.Current} / {unit.HP.max}";
        staminaText.text = $"{unit.Stamina.Current} / {unit.Stamina.max}";
        stressText.text = $"{unit.Stress.Current} / {unit.Stress.max}";
    }

    public void SetInfo()
    {
        characterName.text = unit.Data.Name;
        characterIcon.uvRect
                = GameManager.uiManager.unitRenderTexture.LoadRenderTexture(unit.Data.UnitAssetFileName);
        gradeIcon.sprite = GameManager.uiManager.gradeIcons[(int)unit.UnitGrade];
        characterJob.text = unit.Data.Job.ToString();

        if (unit.Skills.Count >= 1)
        {
            skillIcons.TryGetValue(unit.Skills[0].Data.SkillId, out var value);
            skill1_Icon.image.sprite = value;
            skill1_Icon.onClick.AddListener(
                () => SetSkillPopUp(unit.Skills[0])
                );
            skill1_Name.text = unit.Skills[0].Data.SkillName;
            skill1_Desc.text = unit.Skills[0].Data.SkillDesc;
        }
        if (unit.Skills.Count >= 2)
        {
            skillIcons.TryGetValue(unit.Skills[1].Data.SkillId, out var value);
            skill2_Icon.image.sprite = value;
            skill2_Icon.onClick.AddListener(
                () => SetSkillPopUp(unit.Skills[1])
                );
            skill2_Name.text = unit.Skills[1].Data.SkillName;
            skill2_Desc.text = unit.Skills[1].Data.SkillDesc;
        }

        attributeText.text = unit.Data.BasicAttackType.ToString();

        hpBar.interactable = false;
        hpBar.value = (float)unit.HP.Current / (float)unit.HP.max;
        hpText.text = $"{unit.HP.Current} / {unit.HP.max}";

        staminaBar.interactable = false;
        staminaBar.value = (float)unit.Stamina.Current / (float)unit.Stamina.max;
        staminaText.text = $"{unit.Stamina.Current} / {unit.Stamina.max}";

        stressBar.interactable = false;
        stressBar.value = (float)unit.Stress.Current / (float)unit.Stress.max;
        stressText.text = $"{unit.Stress.Current} / {unit.Stress.max}";

        totalCombatValue.text = unit.CombatPoint.ToString();
        combatDetail.onClick.AddListener(() =>
        {
            combatPopUp.SetActive(true);
            SetCombatPopUp(unit);
        });

        strValue.text = unit.BaseStr.Current.ToString();
        magValue.text = unit.BaseWiz.Current.ToString();
        agiValue.text = unit.BaseAgi.Current.ToString();

        attackSpeedValue.text = unit.AttackSpeed.Current.ToString();
        moveSpeedValue.text = unit.MoveSpeed.Current.ToString();
        critValue.text = unit.CritChance.Current.ToString();
        critDamageValue.text = unit.CritWeight.Current.ToString();
    }

    public void OnButtonPlacement()
    {
        GameManager.uiManager.windows[WINDOW_NAME.CHARACTER_LOCATE].Open();
    }

    public void OnButtonExit()
    {
        GameManager.cameraManager.FinishFocousOnUnit();
        Close();
    }

    private void OnButtonKickOut()
    {
        kickOutPopUp.SetActive(true);
    }

    private void SetSkillPopUp(Skill skill)
    {
        skillPopUp.SetActive(true);
        var skillInfo = skillPopUp.GetComponent<SkillInformation>();
        skillInfo.skillName.text = skill.Data.SkillName;
        skillInfo.skillIcon.sprite = skillIcons.GetValueOrDefault(skill.Data.SkillId);
        skillInfo.skillDesc.text = skill.Data.SkillDesc;
        string cool = "";
        switch (skill.Data.SkillActiveType)
        {
            case SKILL_ACTIVE_TYPE.ALWAYS:
                cool = "패시브 스킬";
                break;
            case SKILL_ACTIVE_TYPE.COOLTIME:
                cool = $"재사용 대기시간 : {skill.Data.SkillActiveValue}초";
                break;
            case SKILL_ACTIVE_TYPE.BASIC_ATTACK_PROBABILITY:
                cool = $"기본 공격 시 {skill.Data.SkillActiveValue * 100}% 확률로 발동";
                break;
            case SKILL_ACTIVE_TYPE.BASIC_ATTACK_COUNT:
                cool = $"기본 공격 {skill.Data.SkillActiveValue}회마다 발동";
                break;
        }
        skillInfo.skillCool.text = cool;
        skillInfo.skillDetailDesc.text = skill.Data.SkillDetail;
    }

    private void SetCombatPopUp(UnitStats unit)
    {
        var combatDetail = combatPopUp.GetComponent<CombatDetailPopUp>();
        combatDetail.totalCombat.text = unit.CombatPoint.ToString();
        string jobBonus = "";
        switch (unit.Data.Job)
        {
            case UNIT_JOB.WARRIOR:
                jobBonus = GameManager.playerManager.warriorWeight.Current.ToString();
                break;
            case UNIT_JOB.MAGICIAN:
                jobBonus = GameManager.playerManager.magicianWeight.Current.ToString();
                break;
            case UNIT_JOB.ARCHER:
                jobBonus = GameManager.playerManager.archerWeight.Current.ToString();
                break;
            default:
                jobBonus = "0";
                break;
        }
        combatDetail.jobCombatBonus.text = $"{jobBonus}%";
        combatDetail.strCombat.text = Mathf.FloorToInt(unit.BaseStr.Current * unit.Data.StrWeight).ToString();
        combatDetail.currStr.text = unit.BaseStr.Current.ToString();
        combatDetail.strApplyAmount.text = $"{unit.Data.StrWeight * 100}%";
        combatDetail.magCombat.text = Mathf.FloorToInt(unit.BaseWiz.Current * unit.Data.WizWeight).ToString();
        combatDetail.currMag.text = unit.BaseWiz.Current.ToString();
        combatDetail.magApplyAmount.text = $"{unit.Data.WizWeight * 100}%";
        combatDetail.agiCombat.text = Mathf.FloorToInt(unit.BaseAgi.Current * unit.Data.AgiWeight).ToString();
        combatDetail.currAgi.text = unit.BaseAgi.Current.ToString();
        combatDetail.agiApplyAmount.text = $"{unit.Data.AgiWeight * 100}%";
    }

    private void TurnOffPopUps()
    {
        skillPopUp.SetActive(false);
        kickOutPopUp.SetActive(false);
        combatPopUp.SetActive(false);
    }
}