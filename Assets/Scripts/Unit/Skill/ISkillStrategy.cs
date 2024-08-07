using UnityEngine;

public interface ISkillStrategy
{
    public Vector3 LastTargetPosition { get; set; }
    public void Use(UnitStats owner, Skill skil);
}