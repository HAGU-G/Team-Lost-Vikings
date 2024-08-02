using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    public GridInfo gridInfo;
    public Dictionary<Vector2Int, Cell> tiles = new Dictionary<Vector2Int, Cell>();
    public GameObject cellPrefab;
    private List<Cell> path;
    

    public List<Cell> usingTileList = new(); //gridMap 내에서 사용 가능한 타일 리스트
    public List<List<Cell>> usableTileList = new(); //usingTileList에 단계별로 할당하기 위한 List


    public Cell GetTile(int x, int y)
    {
        Vector2Int key = new Vector2Int(x, y);
        if (tiles.ContainsKey(key))
            return tiles[key];

        return null;
    }

    private void Awake()
    {
        DrawGrid(gridInfo.col, gridInfo.row);

        InitializeUsableTileList();
        usableTileList.Reverse();
    }

    private void Start()
    {

    }

    private void DrawGrid(int col, int row)
    {
        Cell[,] tileArray = new Cell[col, row];
        Vector3 parentPosition = transform.position;

        for (int x = 0; x < col; x++)
        {
            for (int y = 0; y < row; y++)
            {
                var cellPrefabSize = cellPrefab.GetComponent<SpriteRenderer>().sprite.bounds.size;

                var scaleFactorX = gridInfo.cellSize / cellPrefabSize.x;
                var scaleFactorY = gridInfo.cellSize / cellPrefabSize.y;

                //Vector3 isoPos = new Vector3(
                //    (x - y) * gridInfo.cellSize / 2f,
                //    (x + y) * gridInfo.cellSize / 4f,
                //    0
                //);

                Vector3 isoPos = new Vector3(
                    (x - y) * (cellPrefabSize.x * scaleFactorX) / 2f,
                    (x + y) * (cellPrefabSize.y * scaleFactorY) / (GameSetting.Instance.tileXY * 2f),
                    0
                );

                isoPos += parentPosition;
                GameObject cell = Instantiate(cellPrefab, isoPos, Quaternion.identity, transform);
                var text = cell.GetComponentInChildren<TextMeshPro>();

                SpriteRenderer renderer = cell.GetComponent<SpriteRenderer>();
                Vector2 spriteSize = renderer.sprite.bounds.size;
                //cell.transform.localScale = new Vector3(gridInfo.cellSize / spriteSize.x, gridInfo.cellSize / spriteSize.y, 1);
                cell.transform.localScale = new Vector3(scaleFactorX, scaleFactorY, 1);

                var tile = cell.GetComponent<Cell>();
                tile.tileInfo.id = new Vector2Int(x, y);
                tile.gridMap = this;

                cell.name = $"{tile.tileInfo.id}";
                text.text = $"{tile.tileInfo.id}";
                tiles.Add(tile.tileInfo.id, tile);
                tileArray[x, y] = tile;

            }
        }

        for (int x = 0; x < col; x++)
        {
            for (int y = 0; y < row; y++)
            {
                Cell currentTile = tileArray[x, y];

                if (y > 0) currentTile.adjacentTiles[(int)Sides.Bottom] = (tileArray[x, y - 1]); // Bottom
                if (x < col - 1) currentTile.adjacentTiles[(int)Sides.Right] = (tileArray[x + 1, y]); // Right
                if (x > 0) currentTile.adjacentTiles[(int)Sides.Left] = (tileArray[x - 1, y]); // Left
                if (y < row - 1) currentTile.adjacentTiles[(int)Sides.Top] = (tileArray[x, y + 1]); // Top

                //if (x > 0 && y > 0) currentTile.AdjacentTiles.Add(tileArray[x - 1, y - 1]); // Bottom-Left
                //if (x < col - 1 && y < row - 1) currentTile.AdjacentTiles.Add(tileArray[x + 1, y + 1]); // Top-Right
                //if (x > 0 && y < row - 1) currentTile.AdjacentTiles.Add(tileArray[x - 1, y + 1]); // Top-Left
                //if (x < col - 1 && y > 0) currentTile.AdjacentTiles.Add(tileArray[x + 1, y - 1]); // Bottom-Right
            }
        }
    }

    public void SetUsingTileList(int level)
    {
        usingTileList.Clear();

        if(level > usableTileList.Count)
        {
            Debug.Log("최대로 확장되었습니다.");
            return;
        }

        foreach (var tile in usableTileList[level - 1])
        {
            usingTileList.Add(tile);
            tile.TileColorChange();
        }
    }

    public Vector2Int PosToIndex(Vector3 position)
    {
        var parentPos = gameObject.GetComponentInParent<Transform>().position;
        var localPosition = position - parentPos;

        float x = localPosition.x;
        float y = localPosition.y;

        //float x = position.x - gameObject.transform.position.x;
        //float y = position.y - gameObject.transform.position.y;


        int indexX = Mathf.RoundToInt((GameSetting.Instance.tileXY * y + x) / gridInfo.cellSize);
        int indexY = Mathf.RoundToInt((GameSetting.Instance.tileXY * y - x) / gridInfo.cellSize);

        if (indexX < 0 || indexY < 0 || indexX >= gridInfo.row || indexY >= gridInfo.col)
        {
            //Debug.Log("Out Of Index");
            return new Vector2Int(-1, -1);
        }

        return new Vector2Int(indexX, indexY);
    }

    public Vector3 IndexToPos(Vector2Int index)
    {
        var parentPos = gameObject.GetComponentInParent<Transform>().position;

        int indexX = index.x;
        int indexY = index.y;
        float x = (indexX - indexY) * gridInfo.cellSize / 2f;
        float y = (indexX + indexY) * gridInfo.cellSize / (GameSetting.Instance.tileXY * 2f);

        return new Vector3(x, y, 0) + parentPos;
    }
    private int Heuristic(Cell a, Cell b)
    {
        int ax = a.tileInfo.id.x;
        int ay = a.tileInfo.id.y;
        int bx = b.tileInfo.id.x;
        int by = b.tileInfo.id.y;

        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
    }

    public List<Cell> PathFinding(Cell start, Cell goal)
    {
        path = new List<Cell>();

        var distances = new int[tiles.Count];
        var scores = new int[tiles.Count];

        for (int i = 0; i < distances.Length; ++i)
        {
            distances[i] = scores[i] = int.MaxValue;
            tiles[new Vector2Int(i / gridInfo.row, i % gridInfo.row)].Clear();
        }

        var startId = (start.tileInfo.id.x * gridInfo.row) + start.tileInfo.id.y;
        distances[startId] = 0;
        scores[startId] = Heuristic(start, goal);

        var pq = new PriorityQueue<(int, Cell)>((lhs, rhs) => lhs.Item1.CompareTo(rhs.Item1));
        distances[startId] = 0;
        pq.Enqueue((scores[startId], start));

        while (!pq.IsEmpty)
        {
            var current = pq.Dequeue();
            var currentTile = current.Item2;
            if (currentTile == goal) //찾은 경우
            {
                Cell step = goal;
                while (step != null) //step.previous != null
                {
                    path.Add(step);
                    step = step.previous;
                }
                path.Reverse();

                return path;
            }

            foreach (var adjacentTile in currentTile.adjacentTiles)
            {
                if (adjacentTile == null 
                    //|| !usingTileList.Contains(adjacentTile)
                    || adjacentTile.tileInfo.TileType == TileType.OBJECT
                    //|| adjacentTile.tileInfo.TileType != TileType.ROAD //길 타일 개념 삭제
                    /*|| adjacentTile.tileInfo.Weight == int.MaxValue*/   //가중치 추가되면 수정하기
                    )
                    continue;

                var currentTileId = (currentTile.tileInfo.id.x * gridInfo.row) + currentTile.tileInfo.id.y;
                var adjacentTileId = (adjacentTile.tileInfo.id.x * gridInfo.row) + adjacentTile.tileInfo.id.y;
                var newdistance = distances[currentTileId] /*+ adjacentNode.Weight*/;

                if (newdistance < distances[adjacentTileId])
                {
                    distances[adjacentTileId] = newdistance;
                    scores[adjacentTileId] = distances[adjacentTileId] + Heuristic(adjacentTile, goal);

                    adjacentTile.previous = currentTile;
                    pq.Enqueue((scores[adjacentTileId], adjacentTile));
                }
            }
        }
        return path; //못 찾은 경우
    }

    public (List<Cell>, Cell) GetShortestPath(Cell start, List<Cell>entrances)
    {
        var path = new List<Cell>();
        Cell cell = new();
        path = PathFinding(start, entrances[0]);
        cell = entrances[0];

        foreach(var entrance in entrances)
        {
            var temp = PathFinding(start, entrance);
            if(temp.Count < path.Count)
            {
                path = temp;
                cell = entrance;
            }
        }

        return (path, cell);
    }

    private void InitializeUsableTileList()
    {
        List<Cell> initialTiles = new List<Cell>(tiles.Values);
        usableTileList.Add(new List<Cell>(initialTiles));

        int minRow = gridInfo.minRow - 1, maxRow = gridInfo.row - 1;
        int minCol = gridInfo.minCol - 1, maxCol = gridInfo.col - 1;

        int x = 0, y = 0;

        while (minRow <= maxRow - x && minCol <= maxCol - y)
        {
            //if (minCol >= maxCol - y + 1 && minRow >= maxRow - x)
            //    break;
            //ExcludeTiles(x, maxRow, y, maxCol);
            //y++;
            //if (minCol >= maxCol - y + 1 && minRow >= maxRow - x)
            //    break;
            //ExcludeTiles(x, maxRow, y, maxCol);
            //maxRow--;
            //if (minCol >= maxCol - y + 1 && minRow >= maxRow - x)
            //    break;
            //ExcludeTiles(x, maxRow, y, maxCol);
            //maxCol--;
            //if (minCol >= maxCol - y + 1 && minRow >= maxRow - x)
            //    break;
            //ExcludeTiles(x, maxRow, y, maxCol);
            //x++;

            if (minCol >= maxCol - y + 1 && minRow >= maxRow - x)
                break;
            y++;
            maxRow--;
            maxCol--;
            x++;
            ExcludeTiles(x, maxRow, y, maxCol);
        }
    }

    private void ExcludeTiles(int minRow, int maxRow, int minCol, int maxCol)
    {
        List<Cell> previousList = usableTileList[usableTileList.Count - 1];
        List<Cell> newTiles = new List<Cell>();

        foreach (var tile in previousList)
        {
            if (tile.tileInfo.id.x >= minCol && tile.tileInfo.id.x <= maxCol &&
               tile.tileInfo.id.y >= minRow && tile.tileInfo.id.y <= maxRow)
            {
                newTiles.Add(tile);
            }
        }
        usableTileList.Add(new List<Cell>(newTiles));
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    //    if (num >= usableTileList.Count)
        //    //        return;

        //    //    if (num > 0)
        //    //    {
        //    //        foreach (var i in usableTileList[num - 1])
        //    //        {
        //    //            if (usableTileList[num] == null)
        //    //                return;
        //    //            i.GetComponent<SpriteRenderer>().material.color = Color.white;
        //    //        }
        //    //    }

        //    //    foreach (var t in usableTileList[num])
        //    //    {
        //    //        t.GetComponent<SpriteRenderer>().material.color = Color.blue;
        //    //    }
        //    //    ++num;
        //    //}

        //    if (usingTileList != null)
        //        foreach (var i in usingTileList)
        //        {
        //            i.GetComponent<SpriteRenderer>().material.color = Color.gray;
        //        }
        //}
    }
}
