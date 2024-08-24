using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.AddressableAssets;

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
    private Vector3 targetPos;
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
        //투사체 삭제
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

            if (effect != null)
            {
                effect.Stop();
                effect = null;
            }
            gameObject.SetActive(false);
            Addressables.ReleaseInstance(gameObject);

            return;
        }

        //타겟 검사
        if (!IsFloor)
        {
            if (targetUnit != null
            && targetUnit.gameObject.activeSelf
            && !targetUnit.IsDead
            && owner != null
            && targetUnit.CurrentHuntZone != null
            && targetUnit.CurrentHuntZone.HuntZoneNum == owner.HuntZoneNum)
            {
                direction = (targetUnit.transform.position - transform.position).normalized;
                targetPos = targetUnit.transform.position;
                destination = targetUnit.transform.position;
            }
            else
            {
                targetUnit = null;
            }
        }

        var deltaTime = Time.deltaTime;

        //투사체 움직임
        if (!IsFloor
            && Vector3.Distance(transform.position, destination) <= skillData.ProjectileSpeed * deltaTime)
        {
            SetPosition(destination);
            if (targetUnit == null)
            {
                Remove();
                return;
            }
        }
        else
        {
            SetPosition(transform.position + direction * skillData.ProjectileSpeed * deltaTime);
        }


        //데미지 처리
        int appliedDamage = 0;
        if (IsFloor)
        {
            attackTimer -= deltaTime;
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
                        appliedDamage += ApplyDamage(target);
                }


                if (skillData.VitDrainRatio > 0f && appliedDamage > 0)
                    owner.objectTransform.GetComponent<CombatUnit>()?.TakeHeal(Mathf.FloorToInt(appliedDamage * skillData.VitDrainRatio));

            }

            if (lifeTimer > 0f)
            {
                lifeTimer -= deltaTime;
            }
            else
            {
                Remove();
                return;
            }
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
                    appliedDamage += ApplyDamage(target);

                    if (skillData.VitDrainRatio > 0f && appliedDamage > 0)
                        owner.objectTransform.GetComponent<CombatUnit>()?.TakeHeal(Mathf.FloorToInt(appliedDamage * skillData.VitDrainRatio));

                    Remove();
                    return;
                }
            }
        }
    }

    private int ApplyDamage(CombatUnit target)
    {
        bool isCritical = Random.Range(0, 100) < owner.CritChance.Current;
        var criticalWeight = isCritical ? owner.CritWeight.Current : 1f;
        var critDamage = Mathf.FloorToInt(damage * criticalWeight);

        var damageResult = target.TakeDamage(critDamage, skillData.SkillType, isCritical);
        if (damageResult.Item1 && isDefaultAttack)
            owner.Stress.Current -= GameSetting.Instance.stressReduceAmount;

        return damageResult.Item2;
    }

    private void Remove()
    {
        isActive = false;
    }

    public void SetPosition(Vector3 pos, bool doRotate = true)
    {
        transform.position = pos;
        ellipse.position = transform.position;

        if (effect == null)
            return;

        if (doRotate)
            effect.LookAt(effectPos.position, IsFloor);
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

    public void ResetProjectile(int damage, Vector3 position, Vector3 targetPos, UnitStats owner)
    {
        this.targetPos = targetPos;
        ResetProjectile(damage, position, null, owner);
    }

    public void ResetProjectile(int damage, Vector3 position, CombatUnit targetUnit, UnitStats owner)
    {
        lifeTimer = skillData.SkillDuration;
        attackTimer = 0f;
        this.damage = damage;
        this.owner = owner;

        var combat = owner.objectTransform.GetComponent<CombatUnit>();
        if (combat == null && targetUnit != null)
            targets = targetUnit.Allies;
        else
            targets = combat.Enemies;

        SetPosition(position);

        this.targetUnit = targetUnit;
        if (targetUnit != null)
            targetPos = targetUnit.transform.position;

        destination = targetPos;
        direction = (targetPos - position).normalized;

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
                effect.LookAt(targetPos);
            }
        }
    }
}