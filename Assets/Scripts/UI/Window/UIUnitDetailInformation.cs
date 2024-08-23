using System;
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
    private Dictionary<int, Sprite> gradeIcons = new();
    private Dictionary<int, Sprite> skillIcons = new();

    public GameObject skillPopUp;
    public GameObject kickOutPopUp;
    public GameObject combatPopUp;
    public GameObject recruitPopUp;

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

        var cnt = Enum.GetValues(typeof(UNIT_GRADE)).Length;
        for (int i = 0; i < cnt; ++i)
        {
            var path = $"Grade_0{i + 1}";
            var id = i;
            Addressables.LoadAssetAsync<Sprite>(path).Completed += (obj) => OnGradeIconsLoadDone(obj, id);
        }

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

    private void OnGradeIconsLoadDone(AsyncOperationHandle<Sprite> obj, int id)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            gradeIcons.Add(id, obj.Result);
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
        gradeIcon.sprite = gradeIcons[(int)unit.UnitGrade];
        characterJob.text = unit.Data.Job switch
        {
            UNIT_JOB.MAGICIAN => "법사",
            UNIT_JOB.WARRIOR => "전사",
            UNIT_JOB.ARCHER => "궁수",
            UNIT_JOB.NONE => "용병",
            _ => string.Empty
        };

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

        attributeText.text = unit.Data.BasicAttackType switch
        {
            ATTACK_TYPE.NONE => "고정 공격",
            ATTACK_TYPE.PHYSICAL => "물리 공격",
            ATTACK_TYPE.MAGIC => "마법 공격",
            ATTACK_TYPE.SPECIAL => "특수 공격",
            _ => string.Empty
        };

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

        attackSpeedValue.text = $"{unit.AttackSpeed.Current:0.00}";
        moveSpeedValue.text = $"{unit.MoveSpeed.Current:0.00}";
        critValue.text = $"{unit.CritChance.Current:0}%";
        critDamageValue.text = $"{unit.CritWeight.Current*100f:0}%";


        if(GameManager.unitManager.Waitings.ContainsKey(unit.InstanceID))
        {
            placement.onClick.RemoveAllListeners();
            placement.GetComponentInChildren<TextMeshProUGUI>().text = $"영입";
            placement.onClick.AddListener(OnButtonRecruit);
        }
        else
        {
            placement.onClick.RemoveAllListeners();
            placement.GetComponentInChildren<TextMeshProUGUI>().text = $"배치";
            placement.onClick.AddListener(OnButtonPlacement);
        }
            
    }

    private void OnButtonRecruit()
    {
        GameManager.PlayButtonSFX();
        recruitPopUp.SetActive(true);
        var recruit = recruitPopUp.GetComponent<RecruitPopUp>();
        recruit.exit.onClick.AddListener(() => recruitPopUp.SetActive(false));

        foreach(var u in GameManager.unitManager.Units.Values)
        {
            if(u.Id == unit.Id)
            {
                recruit.text.text = $"같은 모험가를 2명 이상 보유할 수 없습니다. 영입을 원한다면 보유 중인 같은 모험가를 방출해주세요.";
                return;
            }
        }
        
        if(GameManager.unitManager.unitLimitCount < GameManager.unitManager.Units.Count + 1)
        {
            recruit.text.text = $"여관이 꽉 차서 모험가를 더 보유할 수 없습니다. 여관을 업그레이드 하거나 모험가를 방출해 빈 자리를 만들어 주세요.";
            return;
        }
        else
        {
            recruit.text.text = $"모험가가 마을에 합류했습니다.";
            GameManager.unitManager.PickUpCharacter(unit.InstanceID);
            recruit.exit.onClick.RemoveAllListeners();
            recruit.exit.onClick.AddListener(OnButtonExit);
            return;
        }
    }

    public void OnButtonPlacement()
    {
        GameManager.PlayButtonSFX();
        GameManager.uiManager.windows[WINDOW_NAME.CHARACTER_LOCATE].Open();
    }

    public void OnButtonExit()
    {
        GameManager.PlayButtonSFX();
        GameManager.cameraManager.FinishFocusOnUnit();
        Close();
    }

    private void OnButtonKickOut()
    {
        GameManager.PlayButtonSFX();
        //GameManager.uiManager.currentUnitStats = unit;
        Debug.Log(unit.Data.Name);
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
        recruitPopUp.SetActive(false);
    }
}