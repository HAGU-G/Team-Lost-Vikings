﻿using System.Collections.Generic;
using UnityEngine;

public class SkillSingle : ISkillStrategy
{
    Vector3 targetPos;

    public void Use(UnitStats owner, Skill skill, CombatUnit targetUnit)
    {
        var combat = owner.objectTransform.GetComponent<CombatUnit>();

        if (combat == null)
            return;

        if (targetUnit != null)
            targetPos = targetUnit.transform.position;

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
                if (targetUnit != null && !targetUnit.IsDead && targetUnit.gameObject.activeSelf)
                    targetList.Add(targetUnit);
                break;
        }

        if (targetList.Count == 0)
            return;


        //데미지
        int damage = skill.Damage;

        var appliedDamage = 0;
        if (damage > 0)
        {
            bool isCritical = Random.Range(0, 100) < owner.CritChance.Current;
            var criticalWeight = isCritical ? owner.CritWeight.Current : 1f;
            var critDamage = Mathf.FloorToInt(damage * criticalWeight);

            appliedDamage = targetList[0].TakeDamage(critDamage, skill.Data.SkillType, isCritical).Item2;
        }

        //흡혈
        if (skill.Data.VitDrainRatio > 0f && appliedDamage > 0)
            combat.TakeHeal(Mathf.FloorToInt(appliedDamage * skill.Data.VitDrainRatio));

        //버프
        targetList[0].stats.ApplyBuff(new(skill));

        //이펙트
        var effect = GameManager.effectManager.GetEffect(skill.Data.SkillEffectName, SORT_LAYER.OverUnit);
        effect.transform.position = targetList[0].transform.position;
        if (combat.isFlip)
            effect.transform.Rotate(Vector3.up, 180f);
    }

    public void Use(UnitStats owner, Skill skil, Vector3 targetPos)
    {
        this.targetPos = targetPos;
        Use(owner, skil, null);
    }
}