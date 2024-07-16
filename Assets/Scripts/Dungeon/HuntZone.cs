using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class HuntZone : MonoBehaviour
{
    #region INSPECTOR
    public int HuntZoneNumber;
    public UnitOnHunt unitPrefab;
    public Monster monsterPrefab;
    public GameObject regenPointsRoot;
    public GameObject portal;
    public int spawnCount = 1;
    #endregion

    [field: SerializeField] public HuntZoneData data { get; private set; }

    public IObjectPool<Monster> Pool { get; private set; }
    private List<MonsterRegenPoint> regenPoints = new();

    public List<UnitOnHunt> Units { get; private set; } = new();
    public List<Monster> Monsters { get; private set; } = new();
    public bool IsReady { get; private set; }

    private float regenTimer;

    private void Start()
    {
        Ready();
    }

    private void Update()
    {
        if (Monsters.Count >= data.MaxMonNum)
            return;

        regenTimer += Time.deltaTime;
        if (regenTimer >= data.MonRegen)
        {
            regenTimer = 0f;
            SpawnMonster(spawnCount);
        }
    }

    public void Ready()
    {
        Init();
        ResetHuntZone();
    }


    public void Init()
    {
        IsReady = false;

        //데이터 테이블 불러오기
        UpdateRegenPoints();
        SetObjectPool();
    }

    public void ResetHuntZone()
    {
        IsReady = false;

        foreach (var monster in Monsters)
        {
            Pool.Release(monster);
        }

        IsReady = true;
    }

    private void UpdateRegenPoints()
    {
        regenPoints.Clear();
        var points = regenPointsRoot.GetComponentsInChildren<MonsterRegenPoint>();
        foreach (var regenPoint in points)
        {
            if (regenPoints.Contains(regenPoint))
                continue;

            regenPoint.UpdateIndicator();
            regenPoints.Add(regenPoint);
        }
    }

    public void SpawnMonster(int spawnCount)
    {
        var randomPoints = new List<MonsterRegenPoint>();

        foreach (var point in regenPoints)
        {
            if (!point.IsReady)
                continue;

            randomPoints.Add(point);
        }

        while (spawnCount > 0 && randomPoints.Count > 0)
        {

            var point = randomPoints[Random.Range(0, randomPoints.Count)];
            var monster = Pool.Get();

            monster.transform.position = point.transform.position;
            point.Spawned(monster);
            randomPoints.Remove(point);

            spawnCount--;
        }

        if (spawnCount > 0)
        {
            Debug.LogWarning($"{spawnCount}마리 소환 실패 : 리젠 포인트 모두 사용 중");
        }
    }

    private void RemoveNullEntity()
    {
        for (int i = Mathf.Max(Units.Count, Monsters.Count) - 1; i >= 0; i--)
        {
            if (i < Units.Count && Units[i] == null)
                Units.RemoveAt(i);

            if (i < Monsters.Count && Monsters[i] == null)
                Monsters.RemoveAt(i);
        }
    }

    private void OnGUI()
    {
        if (GUILayout.Button("소환"))
        {
            SpawnMonster(1);
        }

        if (GUILayout.Button("죽어"))
        {
            if (Monsters.Count > 0)
                Monsters[0].TakeDamage(Monsters[0].stats.HP.max, ATTACK_TYPE.NONE);
        }

        if (GUILayout.Button("리젠 포인트 갱신"))
        {
            UpdateRegenPoints();
        }
    }




    //오브젝트 풀링
    private void SetObjectPool()
    {
        if (Pool != null)
            return;

        Pool = new LinkedPool<Monster>(
            OnCreateMonster,
            OnGetMonster,
            OnReleaseMonster,
            OnDestroyMonster,
            true, 100
            );

        var preGets = new List<Monster>();
        for (int i = 0; i < data.MaxMonNum; i++)
        {
            preGets.Add(Pool.Get());
        }
        foreach (var monster in preGets)
        {
            Pool.Release(monster);
        }
    }

    private Monster OnCreateMonster()
    {
        var monster = Instantiate(monsterPrefab, transform);
        monster.Init(this);

        return monster;
    }

    private void OnGetMonster(Monster monster)
    {
        monster.ResetMonster();
        monster.gameObject.SetActive(IsReady);

        if (IsReady && !Monsters.Contains(monster))
            Monsters.Add(monster);
    }

    private void OnReleaseMonster(Monster monster)
    {
        monster.gameObject.SetActive(false);

        if (Monsters.Contains(monster))
            Monsters.Remove(monster);

        Debug.Log(Monsters.Count);
    }

    private void OnDestroyMonster(Monster monster) { }
}