using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnitSkills
{
    [field: SerializeField] public List<Skill> SkillList { get; private set; } = new();

    public void SetSkills(params int[] ids)
    {
        SkillList.Clear();
        foreach (var id in ids)
        {
            //TODO 데이터 테이블에서 id 받아와서 할당
            SkillList.Add(null);
        }
    }
    public void SetSkill(int index, int id)
    {
        //TODO 데이터 테이블에서 id 받아와서 할당
        SkillList[index] = null;
    }
    public void SetSkill(int index, Skill skill)
    {
        SkillList[index] = skill;
    }

    public void ResetSkills()
    {
        foreach (var skill in SkillList)
        {
            if (skill == null)
                continue;

            skill.ResetSkill();
        }
    }

}
