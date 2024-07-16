using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class HuntZoneManager : MonoBehaviour
{
    #region INSPECTOR
    public UnitOnHunt unitPrefab;
    public Monster monsterPrefab;
    #endregion

    public Dictionary<int, HuntZone> HuntZones { get; private set; } = new();

    private IObjectPool<Monster> MonsterPool { get; set; }

    private void Awake()
    {
        if (GameManager.huntZoneManager != null)
        {
            Destroy(gameObject);
            return;
        }

        GameManager.huntZoneManager = this;
        SetObjectPool();
    }

    private void SetObjectPool()
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

        monster.gameObject.SetActive(true);
        if (!huntZone.Monsters.Contains(monster))
            huntZone.Monsters.Add(monster);

        return monster;
    }

    public void ReleaseMonster(Monster monster)
    {
        monster.CurrentHuntZone.Monsters.Remove(monster);
        MonsterPool.Release(monster);
    }

    private Monster OnCreateMonster()
    {
        //SceneManager.SetActiveScene(gameObject.scene);
        var monster = Instantiate(monsterPrefab, transform);
        monster.gameObject.SetActive(false);
        monster.Init();

        return monster;
    }

    private void OnGetMonster(Monster monster) { }

    private void OnReleaseMonster(Monster monster)
    {
        monster.gameObject.SetActive(false);
    }

    private void OnDestroyMonster(Monster monster) { }
}
