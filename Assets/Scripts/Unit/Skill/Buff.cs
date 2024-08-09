using UnityEngine;

public struct Buff
{
    public readonly int id;
    public readonly STAT_TYPE type;
    public readonly STAT_VALUE_TYPE valueType;
    private readonly bool isAlways;
    public float Value { get; private set; }
    public float Duration { get; private set; }
    public float Timer { get; private set; }

    public Buff(Skill skill)
    {
        id = skill.Data.SkillId;
        type = skill.Data.BuffType;
        valueType = skill.Data.BuffStatValueType;
        Value = skill.Data.BuffStatValue;
        Duration = skill.Data.SkillDuration;
        Timer = Duration;
        isAlways = skill.Data.SkillActiveType == SKILL_ACTIVE_TYPE.ALWAYS;
    }

    public void Apply(StatInt stat)
    {
        switch (valueType)
        {
            case STAT_VALUE_TYPE.NONE:
                break;
            case STAT_VALUE_TYPE.ADD:
                stat.buffValue += Mathf.FloorToInt(Value);
                break;
            case STAT_VALUE_TYPE.RATIO:
                stat.ratioBuffValue += Value;
                break;
        }
    }

    public void Apply(StatFloat stat)
    {
        switch (valueType)
        {
            case STAT_VALUE_TYPE.NONE:
                break;
            case STAT_VALUE_TYPE.ADD:
                stat.buffValue += Value;
                break;
            case STAT_VALUE_TYPE.RATIO:
                stat.ratioBuffValue += Value;
                break;
        }
    }

    public void Remove(StatInt stat)
    {
        switch (valueType)
        {
            case STAT_VALUE_TYPE.NONE:
                break;
            case STAT_VALUE_TYPE.ADD:
                stat.buffValue -= Mathf.FloorToInt(Value);
                break;
            case STAT_VALUE_TYPE.RATIO:
                stat.ratioBuffValue -= Value;
                break;
        }
    }

    public void Remove(StatFloat stat)
    {
        switch (valueType)
        {
            case STAT_VALUE_TYPE.NONE:
                break;
            case STAT_VALUE_TYPE.ADD:
                stat.buffValue -= Value;
                break;
            case STAT_VALUE_TYPE.RATIO:
                stat.ratioBuffValue -= Value;
                break;
        }
    }

    public void Reset()
    {
        Timer = Duration;
    }

    public void Update(float deltaTime)
    {
        if (!isAlways)
            Timer -= deltaTime;
    }
}