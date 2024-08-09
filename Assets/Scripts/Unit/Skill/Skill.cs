using Newtonsoft.Json;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class Skill
{
    private UnitStats owner;
    private ISkillStrategy skillBehaviour = null;
    public SkillData Data { get; private set; }

    //Save
    public float CurrentActiveValue { get; private set; }

    //Don't Save
    public int Damage => Mathf.FloorToInt(
        owner.CombatPoint * Data.SkillDmgRatio
        + owner.BaseStr.Current * Data.SkillStrRatio
        + owner.BaseWiz.Current * Data.SkillWizRatio
        + owner.BaseAgi.Current * Data.SkillAgiRatio);
    public Ellipse CastEllipse { get; private set; } = null;
    public bool IsReady
    {
        get
        {
            return Data.SkillActiveType switch
            {
                SKILL_ACTIVE_TYPE.NONE => false,
                SKILL_ACTIVE_TYPE.ALWAYS => false,
                SKILL_ACTIVE_TYPE.COOLTIME => (CurrentActiveValue <= 0),
                SKILL_ACTIVE_TYPE.BASIC_ATTACK_PROBABILITY => (Random.value <= Data.SkillActiveValue),
                SKILL_ACTIVE_TYPE.BASIC_ATTACK_COUNT => (CurrentActiveValue >= Data.SkillActiveValue),
                _ => false
            };
        }
    }

    public Skill(SkillData data, UnitStats owner)
    {
        Init(data, owner);
        ResetActiveValue();
    }

    private void Init(SkillData data, UnitStats owner)
    {
        Data = data;
        this.owner = owner;

        CastEllipse ??= new();
        CastEllipse.SetAxies(data.SkillCastRange);
        if (owner.objectTransform != null)
            CastEllipse.position = owner.objectTransform.position;

        skillBehaviour = data.SkillAttackType switch
        {
            SKILL_ATTACK_TYPE.SINGLE => new SkillSingle(),
            SKILL_ATTACK_TYPE.RANGE => new SkillRange(),
            SKILL_ATTACK_TYPE.FLOOR => new SkillFloor(),
            SKILL_ATTACK_TYPE.PROJECTILE => new SkillProjectile(),
            _ => null
        };

        if(data.SkillActiveType == SKILL_ACTIVE_TYPE.ALWAYS)
            owner.ApplyBuff(new(this));
    }

    public void ResetConditionUpdate()
    {
        if (owner.objectTransform == null)
            return;

        var combatUnit = owner.objectTransform.GetComponent<CombatUnit>();

        if (combatUnit == null)
            return;

        switch (Data.SkillActiveType)
        {
            case SKILL_ACTIVE_TYPE.NONE:
                break;

            case SKILL_ACTIVE_TYPE.ALWAYS:
                break;

            case SKILL_ACTIVE_TYPE.COOLTIME:
                combatUnit.OnUpdated += ConditionUpdate;

                break;
            case SKILL_ACTIVE_TYPE.BASIC_ATTACK_PROBABILITY:
                combatUnit.OnAttacked += ConditionUpdate;
                break;

            case SKILL_ACTIVE_TYPE.BASIC_ATTACK_COUNT:
                combatUnit.OnAttacked += ConditionUpdate;
                break;

            default:
                break;
        }
    }

    public void UpdateEllipsePosition(Vector3 pos)
    {
        CastEllipse.position = pos;
    }

    public void Use(Vector3 targetPos)
    {
        skillBehaviour?.Use(owner, this, targetPos);
    }

    private void ConditionUpdate()
    {
        switch (Data.SkillActiveType)
        {
            case SKILL_ACTIVE_TYPE.NONE:
                break;

            case SKILL_ACTIVE_TYPE.ALWAYS:
                break;

            case SKILL_ACTIVE_TYPE.COOLTIME:
                UpdateCoolTime();
                break;

            case SKILL_ACTIVE_TYPE.BASIC_ATTACK_PROBABILITY:
                break;

            case SKILL_ACTIVE_TYPE.BASIC_ATTACK_COUNT:
                CurrentActiveValue += 1f;
                break;

            default:
                break;
        }
    }

    private void UpdateCoolTime()
    {
        if (CurrentActiveValue <= 0f)
            return;

        CurrentActiveValue -= Time.deltaTime;
    }


    public void ResetActiveValue()
    {
        switch (Data.SkillActiveType)
        {
            case SKILL_ACTIVE_TYPE.NONE:
                break;
            case SKILL_ACTIVE_TYPE.ALWAYS:
                break;
            case SKILL_ACTIVE_TYPE.COOLTIME:
                CurrentActiveValue = Data.SkillActiveValue;
                break;
            case SKILL_ACTIVE_TYPE.BASIC_ATTACK_PROBABILITY:
                break;
            case SKILL_ACTIVE_TYPE.BASIC_ATTACK_COUNT:
                CurrentActiveValue = 0;
                break;
            default:
                break;
        }
    }
}