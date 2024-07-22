using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class HuntZoneManager : MonoBehaviour
{
    //TESTCODE
    public UnitStatsData unitStatsData;

    //어드레서블로 교체
    public UnitOnHunt unitPrefab;
    public Monster monsterPrefab;

    public Vector3 offset = Vector3.right * 1000f;

    /// <summary>
    /// Key: 사냥터 ID,
    /// Value: HuntZone
    /// </summary>
    public Dictionary<int, HuntZone> HuntZones { get; private set; } = new();

    private IObjectPool<Monster> MonsterPool { get; set; }
    private IObjectPool<UnitOnHunt> UnitPool { get; set; }

    private void Awake()
    {
        if (GameManager.huntZoneManager != null)
        {
            Destroy(gameObject);
            return;
        }

        GameManager.huntZoneManager = this;

        SetMonsterPool();

        SetUnitPool();
    }


    #region MONSTER_POOL
    private void SetMonsterPool()
    {
        if (MonsterPool != null)
            return;

        MonsterPool = new LinkedPool<Monster>(
            OnCreateMonster,
            OnGetMonster,
            OnReleaseMonster,
            OnDestroyMonster,
            true, 400
            );

        var preGets = new List<Monster>();
        for (int i = 0; i < 30; i++)
        {
            preGets.Add(MonsterPool.Get());
        }
        foreach (var monster in preGets)
        {
            MonsterPool.Release(monster);
        }
    }

    public Monster GetMonster(HuntZone huntZone)
    {
        var monster = MonsterPool.Get();
        monster.ResetMonster(huntZone);

        monster.gameObject.transform.SetParent(huntZone.monstersRoot.transform);
        monster.gameObject.SetActive(true);

        if (!huntZone.Monsters.Contains(monster))
            huntZone.Monsters.Add(monster);

        return monster;
    }

    public void ReleaseMonster(Monster monster)
    {
        monster.CurrentHuntZone.Monsters.Remove(monster);
        monster.OnRelease();
        MonsterPool.Release(monster);
    }

    private Monster OnCreateMonster()
    {
        //SceneManager.SetActiveScene(gameObject.scene);
        var monster = Instantiate(monsterPrefab, transform);
        monster.gameObject.SetActive(false);
        monster.Init();
#if UNITY_EDITOR
        monster.gameObject.AddComponent<EllipseDrawer>();
#endif
        return monster;
    }

    private void OnGetMonster(Monster monster) { }

    private void OnReleaseMonster(Monster monster)
    {
        monster.gameObject.SetActive(false);
        monster.gameObject.transform.SetParent(transform);
    }

    private void OnDestroyMonster(Monster monster) { }
    #endregion

    #region UNIT_POOL
    private void SetUnitPool()
    {
        if (UnitPool != null)
            return;

        UnitPool = new LinkedPool<UnitOnHunt>(
            OnCreateUnit,
            OnGetUnit,
            OnReleaseUnit,
            OnDestroyUnit,
            true, 100
            );

        var preGets = new List<UnitOnHunt>();
        for (int i = 0; i < 10; i++)
        {
            preGets.Add(UnitPool.Get());
        }
        foreach (var unit in preGets)
        {
            UnitPool.Release(unit);
        }
    }

    public UnitOnHunt GetUnitOnHunt(HuntZone huntZone, UnitStats unitStats)
    {
        var unit = UnitPool.Get();
        unit.ResetUnit(unitStats, huntZone);

        unit.stats.ResetStats(); //TESTCODE 회복 기능이 없는 상태에서 테스트 했기 때문에 넣었음.
        unit.gameObject.transform.SetParent(huntZone.unitsRoot.transform);
        unit.gameObject.SetActive(true);

        if (!huntZone.Units.Contains(unit))
            huntZone.Units.Add(unit);

        return unit;
    }

    public void ReleaseUnit(UnitOnHunt unit)
    {
        unit.CurrentHuntZone.Units.Remove(unit);
        unit.OnRelease();
        UnitPool.Release(unit);
    }

    private UnitOnHunt OnCreateUnit()
    {
        var unit = Instantiate(unitPrefab, transform);
        unit.gameObject.SetActive(false);
        unit.Init();
#if UNITY_EDITOR
        unit.gameObject.AddComponent<EllipseDrawer>();
#endif
        return unit;
    }

    private void OnGetUnit(UnitOnHunt unit) { }

    private void OnReleaseUnit(UnitOnHunt unit)
    {
        unit.gameObject.SetActive(false);
        unit.gameObject.transform.SetParent(transform);
    }

    private void OnDestroyUnit(UnitOnHunt unit) { }
    #endregion

    public void AddHuntZone(HuntZone huntZone)
    {
        if (HuntZones.ContainsKey(huntZone.HuntZoneNum))
        {
            Debug.LogError($"사냥터 {huntZone.HuntZoneNum} 이(가) 이미 존재합니다.");
            return;
        }

        HuntZones.Add(huntZone.HuntZoneNum, huntZone);
        huntZone.gameObject.transform.position = offset + Vector3.right * 200f * huntZone.HuntZoneNum;
    }
}
