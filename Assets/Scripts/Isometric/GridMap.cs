using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    public GridInfo gridInfo;
    private int CurrentCol;
    private int CurrentRow;
    public Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();
    public GameObject cellPrefab;


    private void Awake()
    {
        DrawGrid(gridInfo.col, gridInfo.row);
    }


    private void DrawGrid(int col, int row)
    {
        Tile[,] tileArray = new Tile[col, row];

        for (int x = 0; x < col; x++)
        {
            for (int y = 0; y < row; y++)
            {
                Vector3 isoPos = new Vector3(
                    (x - y) * gridInfo.cellSize / 2f,
                    (x + y) * gridInfo.cellSize / 4f,
                    0
                );

                GameObject cell = Instantiate(cellPrefab, isoPos, Quaternion.identity, transform);
                cell.transform.localScale = new Vector3(gridInfo.cellSize, gridInfo.cellSize, gridInfo.cellSize);
                var text = cell.GetComponentInChildren<TextMeshPro>();

                var tile = cell.GetComponent<Tile>();
                tile.tileInfo.id = new Vector2Int(x, y);
                text.text = $"{tile.tileInfo.id}";
                tiles.Add(tile.tileInfo.id, tile);
                tileArray[x, y] = tile;
            }
        }

        for (int x = 0; x < col; x++)
        {
            for (int y = 0; y < row; y++)
            {
                Tile currentTile = tileArray[x, y];

                if (y > 0) currentTile.adjacentTiles[(int)Sides.Bottom] = (tileArray[x, y - 1]); // Bottom
                if (x < col - 1) currentTile.adjacentTiles[(int)Sides.Right] = (tileArray[x + 1, y]); // Right
                if (x > 0) currentTile.adjacentTiles[(int)Sides.Left] = (tileArray[x - 1, y]); // Left
                if (y < row - 1) currentTile.adjacentTiles[(int)Sides.Top] = (tileArray[x, y + 1]); // Top

                //currentTile.UpdateAutoTileId();

                //if (x > 0 && y > 0) currentTile.AdjacentTiles.Add(tileArray[x - 1, y - 1]); // Bottom-Left
                //if (x < col - 1 && y < row - 1) currentTile.AdjacentTiles.Add(tileArray[x + 1, y + 1]); // Top-Right
                //if (x > 0 && y < row - 1) currentTile.AdjacentTiles.Add(tileArray[x - 1, y + 1]); // Top-Left
                //if (x < col - 1 && y > 0) currentTile.AdjacentTiles.Add(tileArray[x + 1, y - 1]); // Bottom-Right
            }
        }
    }

   private void ExpandGrid()
    {
        ++CurrentCol;
        ++CurrentRow;
        
    }

    private void UpdateGrid()
    {
        //tiles[i].UpdateTileInfo(tile);
    }

    public Vector2Int PosToIndex(Vector3 position)
    {
        float x = position.x;
        float y = position.y;

        int indexX = Mathf.RoundToInt((2f * y + x) / gridInfo.cellSize);
        int indexY = Mathf.RoundToInt((2f * y - x) / gridInfo.cellSize);

        if (indexX < 0 || indexY < 0)
        {
            Debug.Log("Out Of Index");
            return new Vector2Int(-1,-1);
        }

        return new Vector2Int(indexX, indexY);
    }

    public Vector3 IndexToPos(Vector2Int index)
    {
        int indexX = index.x;
        int indexY = index.y;
        float x = (indexX - indexY) * gridInfo.cellSize / 2f;
        float y = (indexX + indexY) * gridInfo.cellSize / 4f;

        return new Vector3(x, y, 0);
    }
}
