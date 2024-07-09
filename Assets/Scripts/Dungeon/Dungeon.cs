using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
    public UnitOnDungeon player;
    public UnitOnDungeon monster;

    public List<UnitOnDungeon> players = new();
    public List<UnitOnDungeon> monsters = new();

    private void Awake()
    {
        GameStarter.Instance.SetActiveOnComplete(gameObject);
    }

    private void Update()
    {
        for (int i = Mathf.Max(players.Count, monsters.Count) - 1; i >= 0; i--)
        {
            if (i < players.Count && players[i] == null)
                players.RemoveAt(i);

            if (i < monsters.Count && monsters[i] == null)
                monsters.RemoveAt(i);
        }
    }

    //TESTCODE
    //아래는 모두 테스트코드
    public void Spawn()
    {
        StartCoroutine(CoSpawn());
    }

    IEnumerator CoSpawn()
    {
        for (int i = 0; i < 1; i++)
        {
            var p = Instantiate(player, transform.position + (Vector3)Random.insideUnitCircle * 20f, Quaternion.identity);
            var m = Instantiate(monster, transform.position + (Vector3)Random.insideUnitCircle * 20f, Quaternion.identity);
            p.dungeon = this;
            m.dungeon = this;
            p.Ready();
            m.Ready();
            players.Add(p);
            monsters.Add(m);
            yield return new WaitForEndOfFrame();
        }
    }
}