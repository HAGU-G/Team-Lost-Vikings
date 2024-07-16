using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Pool;

public class HuntZone : MonoBehaviour
{
    #region INSPECTOR
    public int HuntZoneNumber;
    public GameObject regenPointsRoot;
    public GameObject portal;
    public int spawnCount = 1;
    #endregion

    [field: SerializeField] public List<HuntZoneData> HuntZoneDatas { get; private set; }
    [field: SerializeField] public List<MonsterStatsData> MonsterDatas { get; private set; }
    [field: SerializeField] public List<MonsterStatsData> BossDatas { get; private set; }

    private List<MonsterRegenPoint> regenPoints = new();

    public List<UnitOnHunt> Units { get; private set; } = new();
    public List<Monster> Monsters { get; private set; } = new();
    public bool IsReady { get; private set; }

    public int stage = 1;
    public int testStageNum = 1;
    public HuntZoneData CurrentHuntZoneData { get; private set; }
    public MonsterStatsData CurrentMonsterData { get; private set; }
    public MonsterStatsData CurrentBossData { get; private set; }
    private float regenTimer;

    private bool isBossBattle;
    private Monster boss = null;
    private Observer<Monster> bossObserver = new();
    private float bossTimer;
    private float retryTimer;

    private void Start()
    {
        Ready();
    }

    private void Update()
    {
        //보스 몬스터
        if (isBossBattle)
        {
            bossTimer -= Time.deltaTime;
            if (bossTimer <= 0f)
            {
                EndBossBattle(false);
            }
            return;
        }

        //재도전 타이머
        if (retryTimer > 0f)
        {
            retryTimer -= Time.deltaTime;
        }

        //일반 몬스터 스폰
        if (Monsters.Count >= CurrentHuntZoneData.MaxMonNum)
            return;

        regenTimer += Time.deltaTime;
        if (regenTimer >= CurrentHuntZoneData.MonRegen)
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

        bossObserver.OnNotified += ReceiveBossNotify;
    }

    public void ResetHuntZone()
    {
        IsReady = false;

        SetStage(stage);

        regenTimer = 0f;
        for (int i = Monsters.Count - 1; i >= 0; i--)
        {
            Monsters[i].RemoveMonster();
        }

        IsReady = true;
    }

    public void SetStage(int stageNum)
    {
        stage = stageNum;
        var dataIndex = stage - 1;

        CurrentHuntZoneData = HuntZoneDatas[dataIndex];
        CurrentMonsterData = MonsterDatas[dataIndex];
        CurrentBossData = BossDatas[dataIndex];
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

    private List<MonsterRegenPoint> GetActiveRegenPoints()
    {
        var randomPoints = new List<MonsterRegenPoint>();

        foreach (var point in regenPoints)
        {
            if (!point.IsReady)
                continue;

            randomPoints.Add(point);
        }

        return randomPoints;
    }

    public void SpawnMonster(int spawnCount)
    {
        var randomPoints = GetActiveRegenPoints();

        foreach (var point in regenPoints)
        {
            if (!point.IsReady)
                continue;

            randomPoints.Add(point);
        }

        while (spawnCount > 0 && randomPoints.Count > 0)
        {

            var point = randomPoints[Random.Range(0, randomPoints.Count)];
            var monster = GameManager.huntZoneManager.GetMonster(this);

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

    public void StartBossBattle()
    {
        isBossBattle = true;
        Camera.main.backgroundColor = Color.black;

        bossTimer = CurrentHuntZoneData.BossKillTimer;

        var randomPoints = GetActiveRegenPoints();

        boss = GameManager.huntZoneManager.GetMonster(this);
        boss.transform.position = randomPoints[Random.Range(0, randomPoints.Count)].transform.position;
        boss.Subscribe(bossObserver);
    }

    public void EndBossBattle(bool isWin)
    {
        isBossBattle = false;

        bossTimer = 0f;
        Camera.main.backgroundColor = new Color(49 / 255f, 77 / 255f, 121 / 255f);

        if (isWin)
        {
            boss = null;
        }
        else
        {
            boss.RemoveMonster();
            boss = null;
            StartRetryTimer();
        }
    }

    public void ReceiveBossNotify()
    {
        if (bossObserver.LastNotifyType == NOTIFY_TYPE.DEAD)
            EndBossBattle(true);
    }

    public void StartRetryTimer()
    {
        retryTimer = CurrentHuntZoneData.BossRetryTimer;
    }

    // 몬스터와 용병리스트 널 접근 오류 발생 시 이 메서드를 주기적으로 실행
    //private void RemoveNullEntity()
    //{
    //    for (int i = Mathf.Max(Units.Count, Monsters.Count) - 1; i >= 0; i--)
    //    {
    //        if (i < Units.Count && Units[i] == null)
    //            Units.RemoveAt(i);

    //        if (i < Monsters.Count && Monsters[i] == null)
    //            Monsters.RemoveAt(i);
    //    }
    //}

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

        testStageNum = int.Parse(GUILayout.TextField(testStageNum.ToString()));

        if (GUILayout.Button("스테이지 변경") && !isBossBattle)
        {
            SetStage(testStageNum);
            ResetHuntZone();
        }

        GUILayout.Label($"{bossTimer:00} | {retryTimer:00}");
        if (GUILayout.Button("보스 소환") && !isBossBattle && retryTimer <= 0f)
        {
            ResetHuntZone();
            StartBossBattle();
        }
    }
}