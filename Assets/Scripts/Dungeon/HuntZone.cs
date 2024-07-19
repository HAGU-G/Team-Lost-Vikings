using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class HuntZone : MonoBehaviour
{
    #region INSPECTOR
    //public GridMap gridMap;
    //public Construct construct;
    //public GameObject standardBuildingPrefab;

    public GameObject regenPointsRoot;
    public GameObject unitsRoot;
    public GameObject monstersRoot;
    #endregion

    [field: SerializeField] public List<HuntZoneData> HuntZoneDatas { get; private set; }
    [field: SerializeField] public List<MonsterStatsData> MonsterDatas { get; private set; }
    [field: SerializeField] public List<MonsterStatsData> BossDatas { get; private set; }
    public HuntZoneData CurrentHuntZoneData { get; private set; }
    public MonsterStatsData CurrentMonsterData { get; private set; }
    public MonsterStatsData CurrentBossData { get; private set; }

    [field: SerializeField] public int HuntZoneID { get; private set; }
    public bool IsReady { get; private set; }
    public int stage = 1;
    public int testStageNum = 1;

    public List<Monster> Monsters { get; private set; } = new();
    private List<MonsterRegenPoint> regenPoints = new();
    private float regenTimer;

    private Monster boss = null;
    private Observer<Monster> bossObserver = new();
    public bool IsBossBattle { get; private set; }
    public bool CanSpawnBoss { get; private set; } = true;
    public float BossTimer { get; private set; }
    public float RetryTimer { get; private set; }

    public List<UnitOnHunt> Units { get; private set; } = new();

    private void Start()
    {
        //gridMap.gameObject.SetActive(true);
        Init();
        ResetHuntZone();
    }

    private void Update()
    {
        //보스 몬스터
        if (IsBossBattle)
        {
            BossTimer -= Time.deltaTime;

            if (BossTimer <= 0f)
                EndBossBattle(false);

            return;
        }

        //재도전 타이머
        if (RetryTimer <= 0f)
            CanSpawnBoss = true;
        else
            RetryTimer -= Time.deltaTime;

        //일반 몬스터 스폰
        if (Monsters.Count >= CurrentHuntZoneData.MaxMonNum)
            return;

        regenTimer += Time.deltaTime;
        if (regenTimer >= CurrentHuntZoneData.MonRegen)
        {
            regenTimer = 0f;
            SpawnMonster(1);
        }
    }

    public void Init()
    {
        IsReady = false;
        //데이터 테이블 불러오기

        UpdateRegenPoints();

        bossObserver.OnNotified += ReceiveBossNotify;
        GameManager.huntZoneManager.AddHuntZone(this);

        //TESTCODE 기준 타일 설치 - TODO 수정 필요
        //StartCoroutine(CoPlaceStandardBuilding());
    }

    //TESTCODE 기준 타일 설치 - TODO 수정 필요
    //private IEnumerator CoPlaceStandardBuilding()
    //{
    //    yield return new WaitForEndOfFrame();
    //    var maxtile = new Vector2Int(gridMap.gridInfo.row - 1, gridMap.gridInfo.col - 1);
    //    construct.PlaceBuilding(standardBuildingPrefab, gridMap.tiles[maxtile], gridMap);
    //}

    public void ResetHuntZone(bool isRemoveUnit = true)
    {
        IsReady = false;

        SetStage(stage);

        regenTimer = 0f;
        var maxIndex = Mathf.Max(Monsters.Count - 1, Units.Count - 1);
        for (int i = maxIndex; i >= 0; i--)
        {
            if (i < Monsters.Count)
                Monsters[i].RemoveMonster();

            if (isRemoveUnit && i < Units.Count)
                Units[i].RemoveUnit();
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

    public void UpdateRegenPoints()
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
        IsBossBattle = true;
        CanSpawnBoss = false;
        BossTimer = CurrentHuntZoneData.BossKillTimer;

        var randomPoints = GetActiveRegenPoints();

        boss = GameManager.huntZoneManager.GetMonster(this);
        boss.transform.position = randomPoints[Random.Range(0, randomPoints.Count)].transform.position;
        boss.Subscribe(bossObserver);

        foreach (var unit in Units)
        {
            unit.ForceChangeTarget(boss);
        }
    }

    public void EndBossBattle(bool isWin)
    {
        IsBossBattle = false;
        BossTimer = 0f;

        if (isWin)
        {
            boss = null;
            CanSpawnBoss = true;
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
        RetryTimer = CurrentHuntZoneData.BossRetryTimer;
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

    public void KillLastSpawnedMonster()
    {
        if (Monsters.Count > 0)
            Monsters[0].TakeDamage(Monsters[0].stats.HP.max, ATTACK_TYPE.NONE);
    }

    public void SpawnUnit()
    {
        if (Units.Count >= CurrentHuntZoneData.UnitCapacity)
            return;

        foreach (var unitSelected in GameManager.unitManager.Units)
        {
            var unit = unitSelected.Value;
            if (unit.Location != LOCATION.NONE)
                continue;

            var unitOnHunt = GameManager.huntZoneManager.GetUnitOnHunt(this, unit);
            unitOnHunt.transform.position = transform.position;
            Debug.Log(unit.InstanceID + "소환됨", unitOnHunt.gameObject);
            break;
        }
    }

    public void SpawnUnit(int instanceID)
    {
        var unitStat = GameManager.unitManager.GetUnit(instanceID);
        var unitOnHunt = GameManager.huntZoneManager.GetUnitOnHunt(this, unitStat);
        unitOnHunt.transform.position = transform.position;
        Debug.Log(unitStat.InstanceID + "소환됨", unitOnHunt.gameObject);
    }

    public void RemoveUnit(int instanceID)
    {
        Units[Units.FindIndex(
            x =>
            {
                return x.stats.InstanceID == instanceID;
            }
            )].RemoveUnit();
    }
}