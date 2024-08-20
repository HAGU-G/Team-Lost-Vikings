using UnityEngine;
using UnityEngine.AddressableAssets;

public class AttackProjectile : IAttackStrategy
{
    public UnitStats owner = null;
    public SkillData skill = null;
    public Vector3 targetPos;

    public bool Attack(IDamagedable target, int damage, bool isCritical, ATTACK_TYPE type = ATTACK_TYPE.NONE)
    {

        var handle = Addressables.InstantiateAsync("Projectile");
        var proj = handle.WaitForCompletion().GetComponent<Projectile>();

        var targetCombat = target as CombatUnit;
        if(targetCombat != null)
            targetPos = targetCombat.transform.position;

        proj.Init(skill);
        if (targetCombat != null)
        {
            proj.ResetProjectile(
                damage,
                owner.objectTransform.position,
                targetCombat,
                owner);
        }
        else
        {
            proj.ResetProjectile(
               damage,
               owner.objectTransform.position,
               targetPos,
               owner);
        }
        return false;
    }

    public bool Attack(Vector3 targetPos, int damage, bool isCritical, ATTACK_TYPE type = ATTACK_TYPE.NONE)
    {
        this.targetPos = targetPos;
        return Attack(null, damage, isCritical, type);
    }
}
