using System.Collections.Generic;
using UnityEngine;

public class HuntZone : MonoBehaviour
{
    #region INSPECTOR
    public GridMap gridMap;
    public GameObject standardBuildingPrefab;

    public GameObject regenPointsRoot;
    public GameObject unitsRoot;
    public GameObject monstersRoot;
    #endregion

    public Vector3 PortalPos { get; private set; }
    public Construct construct = new();

    /// <summary>
    /// Key: 스테이지, Value: 사냥터 데이터테이블
    /// </summary>
    public Dictionary<int, HuntZoneData> HuntZoneDatas { get; private set; } = new();
    [field: SerializeField] public HuntZoneInfo Info { get; set; }
    public int HuntZoneNum => Info.HuntZoneNum; //기존 코드 유지용
    public int Stage => Info.Stage; //기존 코드 유지용
    public float RetryTimer => Info.RetryTimer; //기존 코드 유지용
    public bool CanSpawnBoss => Info.CanSpawnBoss; //기존 코드 유지용

    public bool IsReady { get; private set; }

    public List<CombatUnit> Units { get; private set; } = new();

    public List<CombatUnit> Monsters { get; private set; } = new();
    private List<MonsterRegenPoint> regenPoints = new();
    private float regenTimer;

    private Monster boss = null;
    private Observer<Monster> bossObserver = new();
    public bool IsBossBattle { get; private set; }
    public float BossTimer { get; private set; }

    public List<Cell> entranceTiles = new();

    private void Awake()
    {
        GameManager.Subscribe(EVENT_TYPE.INIT, OnGameInit);
        GameManager.Subscribe(EVENT_TYPE.START, OnGameStart);
        GameManager.Subscribe(EVENT_TYPE.QUIT, OnGameQUIT);
    }

    private void OnGameInit()
    {
        Init();
        ResetHuntZone(true);
    }

    private void OnGameStart()
    {
        GameManager.huntZoneManager.SetDevelopText(false);
    }

    private void OnGameQUIT()
    {
        if (IsBossBattle)
            EndBossBattle(false);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        //보스 전투 시간
        if (IsBossBattle && BossTimer > 0f)
        {
            BossTimer -= Time.deltaTime;

            if (BossTimer <= 0f)
                EndBossBattle(false);

            return;
        }

        //재도전 타이머
        if (Info.RetryTimer <= 0f && !Info.CanSpawnBoss)
        {
            Info.CanSpawnBoss = true;
            GameManager.huntZoneManager.HuntZoneInfoChanged();
        }
        else
        {
            Info.RetryTimer -= Time.deltaTime;
        }

        //일반 몬스터 스폰
        if (Monsters.Count >= HuntZoneDatas[Info.Stage].MaxMonNum)
            return;

        regenTimer += Time.deltaTime;
        if (regenTimer >= HuntZoneDatas[Info.Stage].MonRegen)
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
            if (data.HuntZoneNum != Info.HuntZoneNum
                || HuntZoneDatas.ContainsKey(data.HuntZoneStage))
                continue;

            HuntZoneDatas.Add(data.HuntZoneStage, data);
        }

        //사냥터 로드
        UpdateRegenPoints();

        bossObserver.OnNotified += ReceiveBossNotify;
        GameManager.huntZoneManager.AddHuntZone(this);

        //타일 설치
        var buildingPos = new Vector2Int(gridMap.gridInfo.row - 2, gridMap.gridInfo.col - 2);
        entranceTiles = construct.PlaceBuilding(standardBuildingPrefab, gridMap.tiles[buildingPos], gridMap)
            .GetComponent<Building>().entranceTiles;
        PortalPos = entranceTiles[Random.Range(0, entranceTiles.Count)].transform.position;
    }

    public void ResetHuntZone(bool isRemoveUnit)
    {
        IsReady = false;

        SetStage(Info.Stage);

        regenTimer = 0f;
        var maxIndex = Mathf.Max(Monsters.Count - 1, Units.Count - 1);
        for (int i = maxIndex; i >= 0; i--)
        {
            if (i < Monsters.Count && (!IsBossBattle || !Monsters[i].stats.isBoss))
                Monsters[i].RemoveUnit();

            //if (IsBossBattle)
            //    EndBossBattle(false);

            if (isRemoveUnit && i < Units.Count)
                (Units[i] as UnitOnHunt).ReturnToVillage();
        }

        IsReady = true;

        GameManager.huntZoneManager.HuntZoneInfoChanged();
    }

    public HuntZoneData GetCurrentData()
    {
        return HuntZoneDatas[Info.Stage];
    }

    public StatsData GetCurrentMonster()
    {
        var id = HuntZoneDatas[Info.Stage].NormalMonsterId;
        var unitTable = DataTableManager.unitTable;
        if (unitTable.ContainsKey(id))
            return unitTable.GetData(id);
        else
            return null;
    }

    public StatsData GetCurrentBoss()
    {
        var id = HuntZoneDatas[Info.Stage].BossMonsterId;
        var unitTable = DataTableManager.unitTable;
        if (unitTable.ContainsKey(id))
            return unitTable.GetData(id);
        else
            return null;
    }

    public void SetStage(int stageNum)
    {
        if (!HuntZoneDatas.ContainsKey(stageNum))
            return;

        if (Info.Stage != stageNum)
            ResetHuntZone(false);

        Info.Stage = stageNum;

        GameManager.huntZoneManager.HuntZoneInfoChanged();
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

            monster.SetPosition(point.transform.position);
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
        Info.CanSpawnBoss = false;
        BossTimer = HuntZoneDatas[Info.Stage].BossTimer;

        var randomPoints = GetActiveRegenPoints();

        boss = GameManager.huntZoneManager.GetMonster(this, true);
        boss.transform.position = randomPoints[Random.Range(0, randomPoints.Count)].transform.position;
        boss.Subscribe(bossObserver);

        foreach (var unit in Units)
        {
            unit.ForceChangeTarget(boss);
        }

        GameManager.huntZoneManager.HuntZoneInfoChanged();
    }

    public void EndBossBattle(bool isWin)
    {
        if (isWin)
        {
            boss = null;
            Info.CanSpawnBoss = true;
            var nextStage = Info.Stage + 1;

            if (HuntZoneDatas.ContainsKey(nextStage))
                SetStage(nextStage);

            var message = GameManager.uiManager.windows[WINDOW_NAME.MESSAGE_POPUP] as UIWindowMessage;
            message.ShowMessage(
                string.Format("승리했습니다!"),
                false,
                1.0f,
                onOpen: () =>
                {
                    var effect = GameManager.effectManager.GetEffect("PopUp_effect", SORT_LAYER.UI);
                    effect.transform.position = message.window.transform.position;
                });
        }
        else
        {
            boss.RemoveUnit();
            boss = null;
            StartRetryTimer();

            var message = GameManager.uiManager.windows[WINDOW_NAME.MESSAGE_POPUP] as UIWindowMessage;
            message.ShowMessage(
                string.Format("패배했습니다..."),
                false,
                1.5f,
                openAnimation: UIWindowMessage.OPEN_ANIMATION.FADEOUT);
        }

        BossTimer = 0f;
        IsBossBattle = false;

        GameManager.huntZoneManager.HuntZoneInfoChanged();
    }

    public void ReceiveBossNotify()
    {
        if (bossObserver.LastNotifyType == NOTIFY_TYPE.DEAD)
            EndBossBattle(true);
    }

    public void StartRetryTimer()
    {
        Info.RetryTimer = HuntZoneDatas[Info.Stage].BossRetryTimer;
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
            Monsters[0].TakeDamage(Monsters[0].stats.HP.max, ATTACK_TYPE.NONE, false);
    }

    public void SpawnUnit(int instanceID)
    {
        var unitStat = GameManager.unitManager.GetUnit(instanceID);
        var unitOnHunt = GameManager.huntZoneManager.GetUnitOnHunt(this, unitStat);
        unitOnHunt.transform.position = PortalPos;
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