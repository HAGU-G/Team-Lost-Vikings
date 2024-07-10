using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class UnitMove : MonoBehaviour
{
    private GridMap gridMap;

    private Tile currentTile;
    private Tile goalTile;

    private float moveTime = 0.5f;
    private float timer;

    private bool isMoving = false;

    List<Tile> path;

    public event Action OnMoveStart;
    public event Action OnMoveEnd;
    public event Action<Tile> OnTile;

    private void Awake()
    {
        gridMap = GameObject.FindWithTag("GridMap").GetComponent<GridMap>();
    }

    private void Update()
    {
        if (!isMoving)
            return;

        timer += Time.deltaTime;
        if(timer >= moveTime)
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
            }
        }
        else
        {
            var start = gridMap.IndexToPos(currentTile.tileInfo.id);
            var end = gridMap.IndexToPos(path[0].tileInfo.id);

            transform.position = Vector3.Lerp(start, end, timer / moveTime);
        }
    }
    
    public bool MoveTo(Tile startTile, Tile target)
    {
        currentTile = startTile;
        if (!target.CanMove)
            return false;

        if(gridMap.PathFinding(startTile, target) == null)
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
