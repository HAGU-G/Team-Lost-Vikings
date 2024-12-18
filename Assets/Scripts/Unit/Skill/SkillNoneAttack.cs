﻿using System.Collections.Generic;
using UnityEngine;

public class SkillNoneAttack : ISkillStrategy
{
    private Ellipse attackEllipse = null;
    private Ellipse buffEllipse = null;

    private Vector3 targetPos;


    public void Use(UnitStats owner, Skill skill, CombatUnit targetUnit)
    {
        if (skill.Data.SkillActiveType == SKILL_ACTIVE_TYPE.ALWAYS)
            return;

        var combat = owner.objectTransform.GetComponent<CombatUnit>();

        if (combat == null)
            return;

        if (targetUnit != null)
            targetPos = targetUnit.transform.position;

        buffEllipse = new(skill.Data.BuffRange, targetPos);

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

    public void Use(UnitStats owner, Skill skil, Vector3 targetPos)
    {
        this.targetPos = targetPos;
        Use(owner, skil, null);
    }
}