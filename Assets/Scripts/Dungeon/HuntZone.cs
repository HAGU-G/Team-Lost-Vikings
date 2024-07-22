using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
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

    [field: SerializeField] public int HuntZoneNum { get; private set; }
    public Dictionary<int, HuntZoneData> HuntZoneDatas { get; private set; } = new();
    public int stage { get; private set; } = 1;
    public bool IsReady { get; private set; }

    public List<UnitOnHunt> Units { get; private set; } = new();

    public List<Monster> Monsters { get; private set; } = new();
    private List<MonsterRegenPoint> regenPoints = new();
    private float regenTimer;

    private Monster boss = null;
    private Observer<Monster> bossObserver = new();
    public bool IsBossBattle { get; private set; }
    public bool CanSpawnBoss { get; private set; } = true;
    public float BossTimer { get; private set; }
    public float RetryTimer { get; private set; }


    private void Start()
    {
        //gridMap.gameObject.SetActive(true);
        Init();
        ResetHuntZone(true);
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
        if (Monsters.Count >= HuntZoneDatas[stage].MaxMonNum)
            return;

        regenTimer += Time.deltaTime;
        if (regenTimer >= HuntZoneDatas[stage].MonRegen)
        {
            regenTimer = 0f;
            SpawnMonster(1);
        }
    }

    public void Init()
    {
        IsReady = false;

        //데이터테이블 로드
        foreach (var data in DataTableManager.huntZoneTable.GetDatas())
        {
            if (data.HuntZoneNum != HuntZoneNum
                || HuntZoneDatas.ContainsKey(data.HuntZoneStage))
                continue;

            HuntZoneDatas.Add(data.HuntZoneStage, data);
        }

        //사냥터 로드
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

    public void ResetHuntZone(bool isRemoveUnit)
    {
        IsReady = false;

        SetStage(stage);

        regenTimer = 0f;
        var maxIndex = Mathf.Max(Monsters.Count - 1, Units.Count - 1);
        for (int i = maxIndex; i >= 0; i--)
        {
            if (i < Monsters.Count && (!IsBossBattle || !Monsters[i].stats.isBoss))
            {
                Monsters[i].RemoveMonster();
            }

            if (isRemoveUnit && i < Units.Count)
                Units[i].RemoveUnit();
        }

        IsReady = true;
    }

    public HuntZoneData GetCurrentData()
    {
        return HuntZoneDatas[stage];
    }

    public MonsterStatsData GetCurrentMonster()
    {
        return DataTableManager.monsterTable.GetData(HuntZoneDatas[stage].NormalMonsterId);
    }

    public MonsterStatsData GetCurrentBoss()
    {
        return DataTableManager.monsterTable.GetData(HuntZoneDatas[stage].BossMonsterId);
    }

    public void SetStage(int stageNum)
    {
        if (stage != stageNum)
            ResetHuntZone(false);

        stage = stageNum;
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
        BossTimer = HuntZoneDatas[stage].BossTimer;

        var randomPoints = GetActiveRegenPoints();

        boss = GameManager.huntZoneManager.GetMonster(this, true);
        boss.transform.position = randomPoints[Random.Range(0, randomPoints.Count)].transform.position;
        boss.Subscribe(bossObserver);

        foreach (var unit in Units)
        {
            unit.ForceChangeTarget(boss);
        }
    }

    public void EndBossBattle(bool isWin)
    {
        if (isWin)
        {
            boss = null;
            CanSpawnBoss = true;
            var nextStage = stage + 1;

            if (HuntZoneDatas.ContainsKey(nextStage))
                SetStage(nextStage);
        }
        else
        {
            boss.RemoveMonster();
            boss = null;
            StartRetryTimer();
        }

        BossTimer = 0f;
        IsBossBattle = false;
    }

    public void ReceiveBossNotify()
    {
        if (bossObserver.LastNotifyType == NOTIFY_TYPE.DEAD)
            EndBossBattle(true);
    }

    public void StartRetryTimer()
    {
        RetryTimer = HuntZoneDatas[stage].BossRetryTimer;
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
        if (Units.Count >= HuntZoneDatas[stage].UnitCapacity)
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