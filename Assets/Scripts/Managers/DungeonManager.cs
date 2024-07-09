using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

//TODO 클래스 내용 모두 테스트코드입니다.
public class DungeonManager : MonoBehaviour
{
    private Dictionary<int, (float, int)> data = new();
    public List<Dungeon> list = new();

    public float frame;
    public int unitCount;

    private void Awake()
    {
        GameStarter.Instance.SetActiveOnComplete(gameObject);
    }

    private void Update()
    {
        frame = 1f / Time.deltaTime;
        unitCount = 0;
        foreach (var dungeon in list)
        {
            unitCount += dungeon.players.Count;
            unitCount += dungeon.monsters.Count;
        }

        if (data.ContainsKey(unitCount))
        {
            var d = data[unitCount];
            data[unitCount] = (d.Item1 + (frame - d.Item1) / (d.Item2 + 1), (d.Item2 + 1));
        }
        else
        {
            data.Add(unitCount, (frame, 1));
        }
    }


    private void OnApplicationQuit()
    {
        if (data == null)
            return;

        var sb = new StringBuilder();
        foreach (var d in data)
        {
            sb.AppendLine($"{d.Key},{d.Value.Item1}");
        }

        //TEST CODE 측정 데이터 파일로 출력하는 부분
        //using (var sw = new StreamWriter(Application.persistentDataPath + "/test.csv"))
        //{
        //    sw.Write(sb);
        //}

        Debug.Log(sb.ToString());
    }

    private void OnGUI()
    {
        GUILayout.Label(frame.ToString());
        GUILayout.Label(unitCount.ToString());
    }

}
