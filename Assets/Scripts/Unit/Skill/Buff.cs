using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class Buff
{
    [JsonProperty] public readonly int id;
    [JsonProperty] public readonly STAT_TYPE type;
    [JsonProperty] public readonly STAT_VALUE_TYPE valueType;
    [JsonProperty] private readonly bool isAlways;
    [JsonProperty] public float Value { get; private set; }
    [JsonProperty] public float Duration { get; private set; }
    [JsonProperty] public float Timer { get; private set; }

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

    [JsonConstructor]
    private Buff() { }

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