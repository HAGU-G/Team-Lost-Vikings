﻿using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.AddressableAssets;
using static UnityEngine.GraphicsBuffer;

public class Projectile : MonoBehaviour
{
    public Transform effectPos;
    private SortingGroup sortingGroup;

    private SkillData skillData;
    private UnitStats owner;
    private List<CombatUnit> targets = null;
    private Ellipse ellipse = null;
    private int damage;

    private Vector3 direction = Vector3.zero;
    private CombatUnit targetUnit;
    private Vector3 destination = Vector3.zero;

    private bool IsFloor => skillData.SkillAttackType == SKILL_ATTACK_TYPE.FLOOR;

    private float lifeTimer;
    private float attackTimer;
    private bool isActive = true;
    private bool isDefaultAttack = false;

    private EffectObject effect;

    private void Awake()
    {
        sortingGroup = GetComponent<SortingGroup>();
        sortingGroup.sortAtRoot = true;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isActive)
        {
            if (!IsFloor)
            {
                var hitEffect = GameManager.effectManager.GetEffect(skillData.SkillEffectName, SORT_LAYER.OverUnit);
                if (hitEffect != null)
                {
                    hitEffect.transform.position = effectPos.position;
                    if (hitEffect.isFlip)
                        hitEffect.transform.Rotate(Vector3.up, 180f);
                }
            }
            gameObject.SetActive(false);
            Addressables.ReleaseInstance(gameObject);
        }

        if (!IsFloor)
        {
            if (targetUnit != null
            && !targetUnit.gameObject.activeSelf
            && !targetUnit.IsDead)
            {
                direction = (targetUnit.transform.position - transform.position).normalized;
                destination = targetUnit.transform.position;
            }
            else
            {
                targetUnit = null;
            }
        }


        SetPosition(transform.position + direction * skillData.ProjectileSpeed * Time.deltaTime);

        lifeTimer -= Time.deltaTime;

        int appliedDamage = 0;

        if (IsFloor)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer < 0f)
            {
                attackTimer += skillData.SkillFloorActiveTerm;
                foreach (var target in targets)
                {
                    if (target.IsDead
                        || !target.gameObject.activeSelf
                        || (skillData.SkillTarget == TARGET_TYPE.OWN && target != owner.objectTransform.GetComponent<CombatUnit>()))
                        continue;

                    if (ellipse.IsCollidedWith(target.stats.SizeEllipse))
                        ApplyDamage(target);
                }
            }

            if (lifeTimer <= 0f)
                Remove();
        }
        else
        {
            sortingGroup.sortingOrder = Mathf.FloorToInt(-transform.position.y);
            foreach (var target in targets)
            {
                if (target.IsDead
                    || !target.gameObject.activeSelf
                    || (skillData.SkillTarget == TARGET_TYPE.OWN && target != owner.objectTransform.GetComponent<CombatUnit>()))
                    continue;

                if (ellipse.IsCollidedWith(target.stats.SizeEllipse))
                {
                    ApplyDamage(target);
                    Remove();
                    break;
                }
            }

            if (Vector3.Distance(transform.position, destination) <= skillData.ProjectileSpeed * Time.deltaTime)
                Remove();
        }

        //흡혈
        if (skillData.VitDrainRatio > 0f && appliedDamage > 0f)
            owner.objectTransform.GetComponent<CombatUnit>()?.TakeHeal(Mathf.FloorToInt(appliedDamage * skillData.VitDrainRatio));
    }

    private int ApplyDamage(CombatUnit target)
    {
        var damageResult = target.TakeDamage(damage, skillData.SkillType);
        if (damageResult.Item1 && isDefaultAttack)
            owner.Stress.Current -= GameSetting.Instance.stressReduceAmount;

        return damageResult.Item2;
    }

    private void Remove()
    {
        isActive = false;

        if (effect == null)
            return;

        effect.Stop();
        effect = null;
    }

    public void SetPosition(Vector3 pos, bool doRotate = true)
    {
        transform.position = pos;
        ellipse.position = transform.position;

        if (effect == null)
            return;

        if (doRotate)
            effect.LookAt(effectPos.position);

        effect.transform.position = effectPos.position;

    }

    public void Init(SkillData skillData, bool isDefaultAttack = false)
    {
        this.skillData = skillData;
        this.isDefaultAttack = isDefaultAttack;

        if (IsFloor)
            sortingGroup.sortingLayerName = "SkillFloor";
        else
            sortingGroup.sortingOrder = Mathf.FloorToInt(-transform.position.y);

        //TESTCODE
        float projectileSize = IsFloor ? skillData.SkillAttackRange : GameSetting.Instance.projectileSize;
        ellipse = new(projectileSize);
    }

    public void ResetProjectile(int damage, Vector3 position, CombatUnit targetUnit, UnitStats owner)
    {
        lifeTimer = skillData.SkillDuration;
        attackTimer = 0f;
        this.damage = damage;
        this.owner = owner;
        targets = owner.objectTransform.GetComponent<CombatUnit>().Enemies;

        SetPosition(position);
        this.targetUnit = targetUnit;
        destination = targetUnit.transform.position;
        direction = (targetUnit.transform.position - position).normalized;

        isActive = true;
        gameObject.SetActive(true);

        effect = GameManager.effectManager.GetEffect(skillData.ProjectileFileName);
        if (effect != null)
        {
            effect.isOnProjectile = true;
            effect.transform.position = effectPos.position;
            effect.isFlip = owner.objectTransform.GetComponent<Unit>().isFlip;

            if (IsFloor)
            {
                effect.transform.localScale = Vector3.one * skillData.SkillAttackRange;
                if (effect.isFlip)
                    effect.transform.Rotate(Vector3.up, 180f);
            }
            else
            {
                //effect.transform.localScale = Vector3.one * GameSetting.Instance.projectileSize;
                effect.LookAt(targetUnit.transform.position);
            }
        }
    }
}