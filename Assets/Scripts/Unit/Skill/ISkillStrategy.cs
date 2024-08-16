public interface ISkillStrategy
{
    public void Use(UnitStats owner, Skill skil, CombatUnit targetUnit);
}