using System.Buffers;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class Skill
{
    private Unit owner;
    private ISkillStrategy skillBehaviour = new SkillTest001();

    public Skill(SkillData data, UnitOnDungeon owner)
    {
        Init(data, owner);
        ResetSkill();
    }

    [field: SerializeField] public SkillData Data { get; private set; }

    //Save
    public float CurrentActiveValue { get; private set; }

    //Don't Save
    public bool IsReady
    {
        get
        {
            return Data.ActiveType switch
            {
                SKILL_ACTIVE_TYPE.NONE => false,
                SKILL_ACTIVE_TYPE.ALWAYS => true,
                SKILL_ACTIVE_TYPE.COOLTIME => (CurrentActiveValue <= 0),
                SKILL_ACTIVE_TYPE.PROBABILITY => (Random.value <= Data.ActiveValue),
                SKILL_ACTIVE_TYPE.ATTACK_COUNT => (CurrentActiveValue >= Data.ActiveValue),
                _ => false
            };
        }
    }


    public void Init(SkillData data, UnitOnDungeon owner)
    {
        SetData(data);
        SetOwner(owner);
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
        var ownerOnDungeon = owner as UnitOnDungeon;
        if (ownerOnDungeon == null)
            return;

        switch (Data.ActiveType)
        {
            case SKILL_ACTIVE_TYPE.NONE:
                break;

            case SKILL_ACTIVE_TYPE.ALWAYS:
                break;

            case SKILL_ACTIVE_TYPE.COOLTIME:
                ownerOnDungeon.OnUpdated += Update;

                break;
            case SKILL_ACTIVE_TYPE.PROBABILITY:
                break;

            case SKILL_ACTIVE_TYPE.ATTACK_COUNT:
                ownerOnDungeon.OnAttacked += Update;
                break;

            default:
                break;
        }
    }

    public void Update()
    {
        ConditionUpdate();

        if (IsReady)
            Use();
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
            case SKILL_ACTIVE_TYPE.PROBABILITY:
                break;

            case SKILL_ACTIVE_TYPE.ATTACK_COUNT:
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
            case SKILL_ACTIVE_TYPE.PROBABILITY:
                break;
            case SKILL_ACTIVE_TYPE.ATTACK_COUNT:
                CurrentActiveValue = 0;
                break;
            default:
                break;
        }
    }
}