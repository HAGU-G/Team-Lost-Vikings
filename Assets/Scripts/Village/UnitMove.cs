using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitMove : MonoBehaviour
{
    private GridMap gridMap;

    private Cell currentTile;
    private Cell goalTile;

    private float moveTime = 1.0f;
    private float timer;

    private bool isMoving = false;
    private bool moveStart = false;

    List<Cell> path;

    public event Action OnMoveStart;
    public event Action OnMoveEnd;
    public event Action<Cell> OnTile;
    public event Action<Cell> OnTargetTile;

    private Vector3 start;
    private Vector3 end;
    private float moveSpeed;
    private UnitOnVillage unitOnVillage;

    private void Awake()
    {
        gridMap = GameObject.FindWithTag("GridMap").GetComponent<GridMap>();
        unitOnVillage = GetComponentInChildren<UnitOnVillage>();
    }

    private void Update()
    {
        if (!isMoving)
            return;
        timer += Time.deltaTime;

        //if (!moveStart)
        //    return;

        //Move();
        end = gridMap.IndexToPos(path[0].tileInfo.id);
        var dis = Vector3.Distance(start, end);
        //Debug.Log(dis);
        if (timer >= (1f * dis / unitOnVillage.stats.MoveSpeed.Current) )
        {
            timer = 0f;
            currentTile = path[0];

            OnTile?.Invoke(currentTile);

            path.RemoveAt(0);
            //var pos = gridMap.IndexToPos(currentTile.tileInfo.id);

            start = transform.position;

            if (path.Count == 0)
            {
                isMoving = false;
                goalTile = null;
                OnMoveEnd?.Invoke();
                OnTargetTile?.Invoke(currentTile);
            }
        }
        else
        {
            // var start = gridMap.IndexToPos(currentTile.tileInfo.id);
            //gameObject.transform.position;

            //end = gridMap.IndexToPos(path[0].tileInfo.id);

            unitOnVillage.SetPosition(
                Vector3.Lerp(
                    start, 
                    end, 
                    timer / (1f * dis / unitOnVillage.stats.MoveSpeed.Current)
                    ));
        }
    }

    public bool MoveTo(Cell startTile, Cell target)
    {
        start = transform.position;


        if (startTile == target)
        {
            currentTile = target;
            OnTargetTile?.Invoke(currentTile);
            return true;
        }

        currentTile = startTile;
        if (!target.CanMove)
            return false;

        path = gridMap.PathFinding(startTile, target);
        if (path == null || path.Count == 0)
        {
            Debug.Log("경로를 찾지 못했습니다.");
            return false;
        }

        path = gridMap.PathFinding(isMoving ? path[0] : startTile, target);
        if (path.Count == 0)
            return false;

        if (!isMoving)
        {
            //path.RemoveAt(0);
            isMoving = true;

            OnMoveStart?.Invoke();
        }

        path.RemoveAt(0);
        timer = 0f;

        //path = gridMap.PathFinding(startTile, target);
        if (path.Count == 0)
            return false;

        //if (!moveStart)
        //{
        //    path.RemoveAt(0);
        //    moveStart = true;
        //    timer = 0f;

        //    OnMoveStart?.Invoke();
        //}

        goalTile = target;
        //var end = gridMap.IndexToPos(path[0].tileInfo.id);
        //transform.position = Vector3.Lerp(start, end, timer / moveTime);

        return true;
    }
}
