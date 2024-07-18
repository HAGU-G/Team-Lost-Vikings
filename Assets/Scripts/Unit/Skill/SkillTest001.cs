using UnityEngine;

public class SkillTest001 : ISkillStrategy
{
    public void Use(Unit owner)
    {
        GameObject.Instantiate(owner.skillEffect,owner.transform.position,Quaternion.identity);
    }
}