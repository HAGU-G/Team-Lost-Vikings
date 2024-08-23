using UnityEngine;

public class UnitSpawnTester : MonoBehaviour
{
//    public TextMeshProUGUI text1;
//    public TextMeshProUGUI text2;

//    public Button buttonReroll;
//    public Button buttonSpawnUnit;
//    public Button buttonSpawnMonster;

//    public UnitStatsData unitData;
//    public MonsterStatsData monsterData;

//    public List<HuntZone> dungeons;
//    public UnitOnHunt unit;
//    public Monster monster;

//    private UnitStats gachaResult = null;

//    public GameObject selectImage;
//    private IStatUsable selected;

//    public TextMeshProUGUI maxHP;
//    public TextMeshProUGUI maxStamina;
//    public TextMeshProUGUI maxStress;

//    public TMP_InputField hp;
//    public TMP_InputField stamina;
//    public TMP_InputField stress;

//    public DateTime lastSpawnTime = DateTime.MinValue;
//    public float spawnInterval = 5f;
//    public int spawnCount = 7;

//    public void Select(IStatUsable select)
//    {
//        selected = select;
//    }

//    private void Awake()
//    {
//        //UnitStats.UpgradeStats.BaseHP = 1000;
//        gachaResult = new UnitStats();
//        gachaResult.InitStats(unitData);
//        gachaResult.ResetStats();


//        buttonReroll.onClick.AddListener(() =>
//        {
//            gachaResult = new UnitStats();
//            gachaResult.InitStats(unitData);
//            gachaResult.ResetStats();

//            var sb = new StringBuilder();
//            //foreach (var item in gachaResult.GetType().GetProperties())
//            //{
//            //    sb.AppendLine($"{item.Name}: {item.GetValue(gachaResult).ToString()}");
//            //}
//            text2.text = sb.ToString();
//        });

//        buttonSpawnUnit.onClick.AddListener(() =>
//        {
//            foreach (var dungeon in dungeons)
//            {
//                var u = Instantiate(unit, dungeon.transform.position, Quaternion.identity);
//                u.gameObject.AddComponent<UnitSelectorTest>().spawner = this;
//                u.stats = gachaResult.Clone();
//                //u.PortalPos = dungeon.transform.position;
//                u.Init();
//                u.ResetUnit(gachaResult.Clone(), dungeon);
//                u.gameObject.AddComponent<EllipseDrawer>();
//                dungeon.Units.Add(u);
//            }
//        });

//        buttonSpawnMonster.onClick.AddListener(SpawnMonster);
//    }

//    private void SpawnMonster()
//    {
//        foreach (var dungeon in dungeons)
//        {
//            var m = Instantiate(monster);
//            m.gameObject.AddComponent<UnitSelectorTest>().spawner = this;
//            m.stats = new();
//            //m.testData = monsterData;
//            m.Ready(dungeon);
//            m.gameObject.AddComponent<EllipseDrawer>();
//            dungeon.Monsters.Add(m);
//        }
//    }

//    private void Start()
//    {
//        SyncedTime.Sync();
//    }

//    private void Update()
//    {
//        if (SyncedTime.IsSynced && SyncedTime.Now >= lastSpawnTime.AddSeconds(spawnInterval))
//        {
//            lastSpawnTime = SyncedTime.Now;

//            for (int i = 0; i < spawnCount; i++)
//            {
//                SpawnMonster();
//            }
//        }

//        text1.text = SyncedTime.IsSynced ? $"{SyncedTime.Now:yyyy-MM-dd HH:mm:ss.fff}
//        : {1f / Time.deltaTime:0.00}" : "Loading...";
//        if (selected != null)
//        {
//            if (selected.StatGroup == STAT_GROUP.UNIT_ON_DUNGEON)
//            {
//                var unitOnDungeon = selected as UnitOnHunt;
//                if (unitOnDungeon == null)
//                    return;

//                selectImage.transform.position = unitOnDungeon.transform.position;

//                maxHP.text = unitOnDungeon.stats.HP.max.ToString();
//                maxStamina.text = unitOnDungeon.stats.Stamina.max.ToString();
//                maxStress.text = unitOnDungeon.stats.Stress.max.ToString();

//                if (!hp.isFocused)
//                    hp.text = unitOnDungeon.stats.HP.ToString();
//                if (!stamina.isFocused)
//                    stamina.text = unitOnDungeon.stats.Stamina.ToString();
//                if (!stress.isFocused)
//                    stress.text = unitOnDungeon.stats.Stress.ToString();
//            }
//            else if (selected.StatGroup == STAT_GROUP.MONSTER)
//            {
//                var monster = selected as Monster;
//                if (monster == null)
//                    return;

//                selectImage.transform.position = monster.transform.position;

//                maxHP.text = monster.stats.HP.max.ToString();
//                maxStamina.text = string.Empty;
//                maxStress.text = string.Empty;

//                if (!hp.isFocused)
//                    hp.text = string.Empty;
//                if (!stamina.isFocused)
//                    stamina.text = string.Empty;
//                if (!stress.isFocused)
//                    stress.text = string.Empty;
//            }

//        }
//        else
//        {
//            maxHP.text = string.Empty;
//            maxStamina.text = string.Empty;
//            maxStress.text = string.Empty;
//            hp.text = string.Empty;
//            stamina.text = string.Empty;
//            stress.text = string.Empty;
//        }
//    }

//    public void SetHp(string text)
//    {
//        if (selected is not UnitOnHunt)
//            return;

//        if (int.TryParse(text, out var hp))
//        {
//            (selected as UnitOnHunt).stats.HP.Current = hp;
//        }
//    }

//    public void SetStamina(string text)
//    {
//        if (selected is not UnitOnHunt)
//            return;

//        if (int.TryParse(text, out var stamina))
//        {
//            (selected as UnitOnHunt).stats.Stamina.Current = stamina;
//        }
//    }

//    public void SetStress(string text)
//    {
//        if (selected is not UnitOnHunt)
//            return;

//        if (int.TryParse(text, out var stress))
//        {
//            (selected as UnitOnHunt).stats.Stress.Current = stress;
//        }
//    }
}
