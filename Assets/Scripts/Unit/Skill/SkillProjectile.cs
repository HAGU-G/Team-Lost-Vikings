using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SkillProjectile : ISkillStrategy
{
    public void Use(UnitStats owner, Skill skill, CombatUnit targetUnit)
    {
        var combat = owner.objectTransform.GetComponent<CombatUnit>();

        if (combat == null)
            return;

        //대상 설정
        List<CombatUnit> targetList = new();
        switch (skill.Data.SkillTarget)
        {
            case TARGET_TYPE.OWN:
                targetList.Add(combat);
                break;

            case TARGET_TYPE.TEAM:
                {
                    var targets = combat.GetCollidedUnit(skill.CastEllipse, combat.Allies.ToArray());
                    foreach (var target in targets)
                    {
                        if (target.Item1.stats == owner
                            || target.Item1.IsDead
                            || !target.Item1.gameObject.activeSelf)
                            continue;

                        targetList.Add(target.Item1);
                        break;
                    }
                }
                break;
            case TARGET_TYPE.TEAM_ALL:
                {
                    var targets = combat.GetCollidedUnit(skill.CastEllipse, combat.Allies.ToArray());
                    foreach (var target in targets)
                    {
                        if (target.Item1.IsDead
                            || !target.Item1.gameObject.activeSelf)
                            continue;

                        targetList.Add(target.Item1);
                        break;
                    }
                }
                break;

            case TARGET_TYPE.ENEMY:
                if (combat.HasTarget())
                    targetList.Add(combat.attackTarget);
                break;
        }

        


        //투사체 생성
        var handle = Addressables.InstantiateAsync("Projectile");
        var proj = handle.WaitForCompletion().GetComponent<Projectile>();
        proj.Init(skill.Data);
        proj.ResetProjectile(
            skill.Damage,
            combat.transform.position,
            targetUnit,
            owner);

        //버프

        if (targetList.Count == 0)
            return;

        targetList[0].stats.ApplyBuff(new(skill));

        //이펙트



    }
}