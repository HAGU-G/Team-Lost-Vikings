using UnityEngine;

public interface ISkillStrategy
{
    public void Use(UnitStats owner, Skill skil, CombatUnit targetUnit);
    public void Use(UnitStats owner, Skill skil, Vector3 targetPos);
}