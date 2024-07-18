using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class UnitMove : MonoBehaviour
{
    private GridMap gridMap;

    private Cell currentTile;
    private Cell goalTile;

    private float moveTime = 0.5f;
    private float timer;

    private bool isMoving = false;

    List<Cell> path;

    public event Action OnMoveStart;
    public event Action OnMoveEnd;
    public event Action<Cell> OnTile;
    public event Action<Cell> OnTargetTile;

    private void Awake()
    {
        gridMap = GameObject.FindWithTag("GridMap").GetComponent<GridMap>();
    }

    private void Update()
    {
        if (!isMoving)
            return;

        timer += Time.deltaTime;

        if (timer >= moveTime)
        {
            timer = 0f;
            currentTile = path[0];
            
            OnTile?.Invoke(currentTile);

            path.RemoveAt(0);
            var pos = gridMap.IndexToPos(currentTile.tileInfo.id);

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
            var start = gridMap.IndexToPos(currentTile.tileInfo.id);
            var end = gridMap.IndexToPos(path[0].tileInfo.id);

            transform.position = Vector3.Lerp(start, end, timer / moveTime);
        }
    }

    public bool MoveTo(Cell startTile, Cell target)
    {
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
            path.RemoveAt(0);
            isMoving = true;
            timer = 0f;

            OnMoveStart?.Invoke();
        }

        goalTile = target;
        return true;
    }
}
