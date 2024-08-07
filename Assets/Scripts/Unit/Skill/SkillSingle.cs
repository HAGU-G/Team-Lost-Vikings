using System.Collections.Generic;
using UnityEngine;

public class SkillSingle : ISkillStrategy
{
    public Vector3 LastTargetPosition { get; set; }

    public void Use(UnitStats owner, Skill skill)
    {
        var combat = owner.objectTransform.GetComponent<CombatUnit>();

        if (combat == null)
            return;

        //대상 설정
        CombatUnit target = null;
        switch (skill.Data.SkillTarget)
        {
            case TARGET_TYPE.OWN:
                target = combat;
                break;

            case TARGET_TYPE.TEAM:
            case TARGET_TYPE.TEAM_ALL:
                {
                    var targets = combat.GetCollidedUnit(skill.CastEllipse, combat.Allies.ToArray());
                    if (targets.Count >= 0)
                        target = targets[0].Item1;
                }
                break;

            case TARGET_TYPE.ENEMY:
                {
                    if (combat == null)
                        break;
                    var targets = combat.GetCollidedUnit(skill.CastEllipse, combat.Enemies.ToArray());
                    if (targets.Count >= 0)
                        target = targets[0].Item1;
                }
                break;
        }

        //데미지
        int damage = Mathf.FloorToInt(owner.CombatPoint * skill.Data.SkillDmgRatio
            + owner.BaseStr.Current * skill.Data.SkillStrRatio
            + owner.BaseWiz.Current * skill.Data.SkillWizRatio
            + owner.BaseAgi.Current * skill.Data.SkillAgiRatio);
        var appliedDamage = target.TakeDamage(damage, skill.Data.SkillType).Item2;


        //흡혈
        combat.TakeHeal(Mathf.FloorToInt(appliedDamage * skill.Data.VitDrainRatio));

        //버프


        //이펙트
    }
}