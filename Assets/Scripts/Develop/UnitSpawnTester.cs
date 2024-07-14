using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DateTime = System.DateTime;

public class UnitSpawnTester : MonoBehaviour
{
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;

    public Button buttonReroll;
    public Button buttonSpawnUnit;
    public Button buttonSpawnMonster;

    public UnitStatsData dataTable;

    public List<Dungeon> dungeons;
    public UnitOnDungeon unit;
    public UnitOnDungeon monster;

    private UnitStatsVariable gachaResult;

    public GameObject selectImage;
    private UnitOnDungeon selected;

    public TextMeshProUGUI maxHP;
    public TextMeshProUGUI maxStamina;
    public TextMeshProUGUI maxStress;

    public TMP_InputField hp;
    public TMP_InputField stamina;
    public TMP_InputField stress;

    public DateTime lastSpawnTime = DateTime.MinValue;
    public float spawnInterval = 5f;
    public int spawnCount = 7;

    public void Select(UnitOnDungeon select)
    {
        selected = select;
    }

    private void Awake()
    {
        GameStarter.Instance.SetActiveOnComplete(gameObject);

        buttonReroll.onClick.AddListener(() =>
        {
            gachaResult = UnitStats.GachaStats(dataTable);

            var sb = new StringBuilder();
            foreach (var item in gachaResult.GetType().GetProperties())
            {
                sb.AppendLine($"{item.Name}: {item.GetValue(gachaResult).ToString()}");
            }
            text2.text = sb.ToString();
        });

        buttonSpawnUnit.onClick.AddListener(() =>
        {
            foreach (var dungeon in dungeons)
            {
                var u = Instantiate(unit, dungeon.portal.transform.position, Quaternion.identity);
                u.gameObject.AddComponent<UnitSelectorTest>().spawner = this;
                u.dungeon = dungeon;
                u.stats = new UnitStats(dataTable, gachaResult.Clone());
                u.destinationPos = dungeon.portal.transform.position;
                u.Ready();
                u.gameObject.AddComponent<EllipseDrawer>();
                dungeon.players.Add(u);
            }
        });

        buttonSpawnMonster.onClick.AddListener(SpawnMonster);
    }

    private void SpawnMonster()
    {
        foreach (var dungeon in dungeons)
        {
            var m = Instantiate(monster, dungeon.portal2.transform.position, Quaternion.identity);
            m.gameObject.AddComponent<UnitSelectorTest>().spawner = this;
            m.dungeon = dungeon;
            m.stats = new UnitStats(dataTable);
            m.destinationPos = dungeon.portal2.transform.position;
            m.Ready();
            m.gameObject.AddComponent<EllipseDrawer>();
            dungeon.monsters.Add(m);
        }
    }

    private void Start()
    {
        SyncedTime.Sync();
    }

    private void Update()
    {
        if (gachaResult != null
            && SyncedTime.IsSynced
            && SyncedTime.Now >= lastSpawnTime.AddSeconds(spawnInterval))
        {
            lastSpawnTime = SyncedTime.Now;

            for (int i = 0; i < spawnCount; i++)
            {
                SpawnMonster();
            }
        }

        text1.text = SyncedTime.IsSynced ? $"{SyncedTime.Now:yyyy-MM-dd HH:mm:ss.fff} FPS : {1f / Time.deltaTime:0.00}" : "Loading...";
        if (selected != null)
        {
            selectImage.transform.position = selected.transform.position;

            maxHP.text = selected.stats.CurrentMaxHP.ToString();
            maxStamina.text = selected.stats.CurrentMaxStamina.ToString();
            maxStress.text = selected.stats.CurrentMaxStress.ToString();

            if (!hp.isFocused)
                hp.text = selected.stats.CurrentHP.ToString();
            if (!stamina.isFocused)
                stamina.text = selected.stats.CurrentStamina.ToString();
            if (!stress.isFocused)
                stress.text = selected.stats.CurrentStress.ToString();
        }
        else
        {
            maxHP.text = string.Empty;
            maxStamina.text = string.Empty;
            maxStress.text = string.Empty;
            hp.text = string.Empty;
            stamina.text = string.Empty;
            stress.text = string.Empty;
        }
    }

    public void SetHp(string text)
    {
        if (selected == null)
            return;

        if (int.TryParse(text, out var hp))
        {
            selected.stats.CurrentHP = hp;
        }
    }

    public void SetStamina(string text)
    {
        if (selected == null)
            return;

        if (int.TryParse(text, out var stamina))
        {
            selected.stats.CurrentStamina = stamina;
        }
    }

    public void SetStress(string text)
    {
        if (selected == null)
            return;

        if (int.TryParse(text, out var stress))
        {
            selected.stats.CurrentStress = stress;
        }
    }
}
