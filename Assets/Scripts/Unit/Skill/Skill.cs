using UnityEngine;

[System.Serializable]
public class Skill
{
    private Unit owner;
    private ISkillStrategy skillBehaviour = new SkillTest001();

    public Skill(SkillData data, UnitOnHunt owner)
    {
        Init(data, owner);
        ResetSkill();
    }

    [field: SerializeField] public SkillData Data { get; private set; }

    //Save
    public float CurrentActiveValue { get; private set; }

    //Don't Save
    public Ellipse CastEllipse { get; private set; }
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


    public void Init(SkillData data, UnitOnHunt owner)
    {
        SetData(data);
        SetOwner(owner);

        CastEllipse ??= new();
        CastEllipse.SetAxies(data.CastRange, owner.transform.position);
    }

    public void SetData(SkillData data)
    {
        Data = data;
    }

    public void SetOwner(Unit owner)
    {
        this.owner = owner;
    }

    private void SetConditionUpdate()
    {
        var unit = owner as UnitOnHunt;
        if (unit == null)
            return;

        unit.OnUpdated += () =>
        {
            CastEllipse.position = unit.transform.position;
        };

        switch (Data.ActiveType)
        {
            case SKILL_ACTIVE_TYPE.NONE:
                break;

            case SKILL_ACTIVE_TYPE.ALWAYS:
                break;

            case SKILL_ACTIVE_TYPE.COOLTIME:
                unit.OnUpdated += ConditionUpdate;

                break;
            case SKILL_ACTIVE_TYPE.BASIC_ATTACK_PROBABILITY:
                unit.OnAttacked += ConditionUpdate;
                break;

            case SKILL_ACTIVE_TYPE.BASIC_ATTACK_COUNT:
                unit.OnAttacked += ConditionUpdate;
                break;

            default:
                break;
        }
    }

    public void Use()
    {
        skillBehaviour.Use(owner);
        ResetActiveValue();
    }

    public virtual void ConditionUpdate()
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

    public virtual void ResetSkill()
    {
        ResetActiveValue();
        SetConditionUpdate();
    }

    private void ResetActiveValue()
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