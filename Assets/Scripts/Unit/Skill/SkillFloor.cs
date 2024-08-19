using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SkillFloor : ISkillStrategy
{
    private Ellipse attackEllipse = null;
    private Ellipse buffEllipse = null;
    private Vector3 targetPos;

    public void Use(UnitStats owner, Skill skill, CombatUnit targetUnit)
    {
        var combat = owner.objectTransform.GetComponent<CombatUnit>();

        if (combat == null)
            return;

        if (targetUnit != null)
            targetPos = targetUnit.transform.position;

        //범위 설정
        attackEllipse = new(skill.Data.SkillAttackRange, targetPos);
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



        //데미지
        int damage = skill.Damage;

        var appliedDamage = 0;
        if (damage > 0)
        {
            foreach (var target in targetList)
            {
                bool isCritical = Random.Range(0, 100) < owner.CritChance.Current;
                var criticalWeight = isCritical ? owner.CritWeight.Current : 1f;
                var critDamage = Mathf.FloorToInt(damage * criticalWeight);
                appliedDamage += target.TakeDamage(critDamage, skill.Data.SkillType, isCritical).Item2;
            }
        }


        //흡혈
        if (skill.Data.VitDrainRatio > 0f && appliedDamage > 0)
            combat.TakeHeal(Mathf.FloorToInt(appliedDamage * skill.Data.VitDrainRatio));

        //버프
        foreach (var target in targetList)
        {
            target.stats.ApplyBuff(new(skill));
        }

        //장판 생성
        var handle = Addressables.InstantiateAsync("Projectile");
        var proj = handle.WaitForCompletion().GetComponent<Projectile>();
        proj.Init(skill.Data);
        if (targetUnit != null)
        {
            proj.ResetProjectile(
            Mathf.FloorToInt(skill.Damage * skill.Data.SkillFloorDmgRatio),
            targetPos,
            targetUnit,
            owner);
        }
        else
        {
            proj.ResetProjectile(
            Mathf.FloorToInt(skill.Damage * skill.Data.SkillFloorDmgRatio),
            targetPos,
            targetPos,
            owner);
        }
        //이펙트
        //TODO addressable 수정 필요 - 오브젝트 풀이나 미리 로드하는 방식 사용
        var effect = GameManager.effectManager.GetEffect(skill.Data.SkillEffectName, SORT_LAYER.OverUnit);
        effect.transform.position = targetPos;
        if (combat.isFlip)
            effect.transform.Rotate(Vector3.up, 180f);
    }

    public void Use(UnitStats owner, Skill skil, Vector3 targetPos)
    {
        this.targetPos = targetPos;
        Use(owner, skil, null);
    }
}