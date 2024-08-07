using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class Skill
{
    private UnitStats owner;
    private ISkillStrategy skillBehaviour = new SkillSingle();

    public Skill(SkillData data, UnitStats owner)
    {
        Init(data, owner);
        ResetActiveValue();
    }

    [field: SerializeField] public SkillData Data { get; private set; }

    //Save
    public float CurrentActiveValue { get; private set; }

    //Don't Save
    public Ellipse CastEllipse { get; private set; } = null;
    public bool IsReady
    {
        get
        {
            return Data.ActiveType switch
            {
                SKILL_ACTIVE_TYPE.NONE => false,
                SKILL_ACTIVE_TYPE.ALWAYS => true,
                SKILL_ACTIVE_TYPE.COOLTIME => (CurrentActiveValue <= 0),
                SKILL_ACTIVE_TYPE.BASIC_ATTACK_PROBABILITY => (Random.value <= Data.ActiveValue),
                SKILL_ACTIVE_TYPE.BASIC_ATTACK_COUNT => (CurrentActiveValue >= Data.ActiveValue),
                _ => false
            };
        }
    }


    private void Init(SkillData data, UnitStats owner)
    {
        Data = data;
        this.owner = owner;

        CastEllipse ??= new();
        CastEllipse.SetAxies(data.CastRange);
        if (owner.objectTransform != null)
            CastEllipse.position = owner.objectTransform.position;
    }

    public void ResetConditionUpdate()
    {
        if (owner.objectTransform == null)
            return;

        var combatUnit = owner.objectTransform.GetComponent<CombatUnit>();

        if (combatUnit == null)
            return;

        switch (Data.ActiveType)
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

    public void Use()
    {
        skillBehaviour.Use(owner, this);
    }

    private void ConditionUpdate()
    {
        switch (Data.ActiveType)
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
        switch (Data.ActiveType)
        {
            case SKILL_ACTIVE_TYPE.NONE:
                break;
            case SKILL_ACTIVE_TYPE.ALWAYS:
                break;
            case SKILL_ACTIVE_TYPE.COOLTIME:
                CurrentActiveValue = Data.ActiveValue;
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