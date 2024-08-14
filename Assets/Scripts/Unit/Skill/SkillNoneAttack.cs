using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SkillNoneAttack : ISkillStrategy
{
    private Ellipse attackEllipse = null;
    private Ellipse buffEllipse = null;



    public void Use(UnitStats owner, Skill skill, Vector3 targetPos)
    {
        if (skill.Data.SkillActiveType == SKILL_ACTIVE_TYPE.ALWAYS)
            return;

        var combat = owner.objectTransform.GetComponent<CombatUnit>();

        if (combat == null)
            return;

        buffEllipse = new(skill.Data.BuffRange, combat.attackTarget.transform.position);

        //대상 설정
        List<CombatUnit> targetList = new();
        switch (skill.Data.SkillTarget)
        {
            case TARGET_TYPE.OWN:
                targetList.Add(combat);
                break;

            case TARGET_TYPE.TEAM:
                {
                    var targets = combat.GetCollidedUnit(buffEllipse, combat.Allies.ToArray());
                    foreach (var target in targets)
                    {
                        if (target.Item1.stats == owner
                            || target.Item1.IsDead
                            || !target.Item1.gameObject.activeSelf)
                            continue;
                        targetList.Add(target.Item1);
                    }
                }
                break;
            case TARGET_TYPE.TEAM_ALL:
                {
                    var targets = combat.GetCollidedUnit(buffEllipse, combat.Allies.ToArray());
                    foreach (var target in targets)
                    {
                        if (target.Item1.IsDead
                            || !target.Item1.gameObject.activeSelf)
                            continue;

                        targetList.Add(target.Item1);
                    }
                }
                break;

            case TARGET_TYPE.ENEMY:
                {
                    if (combat == null)
                        break;
                    var targets = combat.GetCollidedUnit(attackEllipse, combat.Enemies.ToArray());
                    foreach (var target in targets)
                    {
                        if (target.Item1.IsDead
                            || !target.Item1.gameObject.activeSelf)
                            continue;

                        targetList.Add(target.Item1);
                    }
                }
                break;
        }

        if (targetList.Count == 0)
            return;

        //버프
        foreach (var target in targetList)
        {
            target.stats.ApplyBuff(new(skill));
        }

        foreach (var target in targetList)
        {
            var effect = GameManager.effectManager.GetEffect(skill.Data.SkillEffectName, SORT_LAYER.OverUnit);
            effect.transform.position = target.transform.position;
            if (combat.isFlip)
                effect.transform.Rotate(Vector3.up, 180f);
        }
    }
}