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

    public Image skill1_Icon;
    public TextMeshProUGUI skill1_Name;
    public TextMeshProUGUI skill1_Desc;
    public Image skill2_Icon;
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

    private void OnEnable()
    {
        if (!IsReady)
            return;
        unit = GameManager.uiManager.currentUnitStats;
        SetInfo();
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
                if ((skillName == string.Empty || skillName == "0"))
                {
                    Debug.LogWarning($"{skillName} 스킬 이름이 존재하지 않습니다.");
                    continue;
                }
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
            skill1_Icon.sprite = value;
            skill1_Name.text = unit.Skills[0].Data.SkillName;
            skill1_Desc.text = unit.Skills[0].Data.SkillDesc;
        }
        if (unit.Skills.Count >= 2)
        {
            skillIcons.TryGetValue(unit.Skills[1].Data.SkillId, out var value);
            skill2_Icon.sprite = value;
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
        //수정 예정
        GameManager.unitManager.DiscardCharacter(unit.InstanceID);
        Close();
    }
}