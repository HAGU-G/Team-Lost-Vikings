using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class UnitSpawnTester : MonoBehaviour
{
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;

    public Button buttonReroll;
    public Button buttonSpawnUnit;
    public Button buttonSpawnMonster;

    public UnitStatsData dataTable;

    public Dungeon dungeon;
    public UnitOnDungeon unit;
    public UnitOnDungeon monster;

    private UnitStatsVariable gachaResult;

    private void Awake()
    {
        GameStarter.Instance.SetActiveOnComplete(gameObject);

        buttonReroll.onClick.AddListener(() =>
        {
            gachaResult = UnitStats.GachaStats(dataTable);

            var sb = new StringBuilder();
            foreach(var item in gachaResult.GetType().GetProperties())
            {
                sb.AppendLine($"{item.Name}: {item.GetValue(gachaResult).ToString()}");
            }
            text2.text = sb.ToString();
        });

        buttonSpawnUnit.onClick.AddListener(() =>
        {
            var stats = new UnitStats(dataTable, gachaResult.Clone());

            var u = Instantiate(unit, dungeon.transform.position + (Vector3)Random.insideUnitCircle * 10f, Quaternion.identity);
            u.dungeon = dungeon;
            u.stats = stats;
            u.Ready();
            dungeon.players.Add(u);
        });

        buttonSpawnMonster.onClick.AddListener(() =>
        {
            var stats = new UnitStats(dataTable, gachaResult.Clone());

            var m = Instantiate(monster, dungeon.transform.position + (Vector3)Random.insideUnitCircle * 10f, Quaternion.identity);
            m.dungeon = dungeon;
            m.stats = stats;
            m.Ready();
            dungeon.monsters.Add(m);
        });
    }

    private void Start()
    {
        SyncedTime.Sync();
    }

    private void FixedUpdate()
    {
        if (SyncedTime.IsSynced && !SyncedTime.IsSyncing)
        {
            if (!SyncedTime.IsMillisecondSynced)
            {
                SyncedTime.Sync();
            }
            else
            {
                var now = SyncedTime.Now;
                text1.text = $"{now:yyyy-MM-dd HH:mm:ss:fff}";
            }
        }
    }

}
