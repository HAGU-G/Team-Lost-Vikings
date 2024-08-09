using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.AddressableAssets;

public class Projectile : MonoBehaviour
{
    public GameObject tempImage;
    private SortingGroup sortingGroup;

    private SkillData skillData;
    private CombatUnit owner;
    private List<CombatUnit> targets = null;
    private Ellipse ellipse = null;
    private int damage;

    private Vector3 direction = Vector3.zero;
    private Vector3 destination;

    private bool IsFloor => skillData.SkillAttackType == SKILL_ATTACK_TYPE.FLOOR;

    private float lifeTimer;
    private float attackTimer;
    private bool isActive = true;

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
            gameObject.SetActive(false);
            Addressables.ReleaseInstance(gameObject);
        }

        transform.position += direction * skillData.ProjectileSpeed * Time.deltaTime;
        ellipse.position = transform.position;

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
                        || (skillData.SkillTarget == TARGET_TYPE.OWN && target != owner))
                        continue;

                    if (ellipse.IsCollidedWith(target.stats.SizeEllipse))
                        appliedDamage += target.TakeDamage(damage, skillData.SkillType).Item2;
                }
            }

            if (lifeTimer <= 0f)
                isActive = false;
        }
        else
        {
            sortingGroup.sortingOrder = Mathf.FloorToInt(-transform.position.y);
            foreach (var target in targets)
            {
                if (target.IsDead
                    || !target.gameObject.activeSelf
                    || (skillData.SkillTarget == TARGET_TYPE.OWN && target != owner))
                    continue;

                if (ellipse.IsCollidedWith(target.stats.SizeEllipse))
                {
                    appliedDamage += target.TakeDamage(damage, skillData.SkillType).Item2;
                    isActive = false;
                    break;
                }
            }

            if (Vector3.Distance(transform.position, destination) <= skillData.ProjectileSpeed * Time.deltaTime)
                isActive = false;
        }

        //흡혈
        if(skillData.VitDrainRatio > 0f && appliedDamage > 0f)
                owner.TakeHeal(Mathf.FloorToInt(appliedDamage * skillData.VitDrainRatio));
    }

    public void Init(SkillData skillData)
    {
        this.skillData = skillData;


        if (IsFloor)
            sortingGroup.sortingLayerName = "SkillFloor";
        else
            sortingGroup.sortingOrder = Mathf.FloorToInt(-transform.position.y);

        //TESTCODE
        float projectileSize = IsFloor ? skillData.SkillAttackRange : GameSetting.Instance.projectileSize;
        ellipse = new(projectileSize);

        tempImage.transform.localScale = new(
            projectileSize * 2f,
            projectileSize * 2f * GameSetting.Instance.ellipseRatio,
            1f);
    }

    public void ResetProjectile(int damage, Vector3 position, Vector3 destination, CombatUnit owner)
    {
        lifeTimer = skillData.SkillDuration;
        attackTimer = 0f;
        this.damage = damage;
        this.owner = owner;
        targets = owner.Enemies;

        transform.position = position;
        ellipse.position = position;
        this.destination = destination;
        direction = (destination - position).normalized;

        isActive = true;
        gameObject.SetActive(true);
    }
}