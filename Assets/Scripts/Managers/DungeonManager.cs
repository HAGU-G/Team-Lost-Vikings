using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public UnitOnDungeon player;
    public UnitOnDungeon monster;

    public List<UnitOnDungeon> players = new();
    public List<UnitOnDungeon> monsters = new();

    public int unitCount;

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


    public void Spawn()
    {
        StartCoroutine(CoSpawn());
    }

    IEnumerator CoSpawn()
    {
        for (int i = 0; i < 30; i++)
        {
            unitCount++;
            unitCount++;
            var p = Instantiate(player, transform.position + (Vector3)Random.insideUnitCircle * 20f, Quaternion.identity);
            var m = Instantiate(monster, transform.position + (Vector3)Random.insideUnitCircle * 20f, Quaternion.identity);
            p.dungeonManager = this;
            m.dungeonManager = this;
            players.Add(p);
            monsters.Add(m);
            yield return new WaitForEndOfFrame();
        }
    }
}