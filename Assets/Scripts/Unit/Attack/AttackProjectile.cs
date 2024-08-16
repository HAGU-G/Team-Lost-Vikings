using UnityEngine.AddressableAssets;

public class AttackProjectile : IAttackStrategy
{
    public UnitStats owner = null;
    public SkillData skill = null;

    public bool Attack(IDamagedable target, int damage, ATTACK_TYPE type = ATTACK_TYPE.NONE)
    {
        var targetCombat = target as CombatUnit;

        var handle = Addressables.InstantiateAsync("Projectile");
        var proj = handle.WaitForCompletion().GetComponent<Projectile>();


        proj.Init(skill);
        proj.ResetProjectile(
            damage,
            owner.objectTransform.position,
            targetCombat,
            owner);

        return false;
    }
}
