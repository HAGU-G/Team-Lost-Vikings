using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public GridInfo gridInfo;
    private int CurrentCol;
    private int CurrentRow;
    private List<GameObject> tiles = new List<GameObject>();
    public GameObject cellPrefab;


    private void Awake()
    {
        DrawGrid(gridInfo.col, gridInfo.row);
    }

    private void DrawGrid(int col, int row)
    {
        for (int x = 0; x < col; x++)
        {
            for (int y = 0; y < row; y++)
            {
                Vector3 isoPos = new Vector3(
                    (x - y) * gridInfo.cellSize / 2f,
                    (x + y) * gridInfo.cellSize / 4f,
                    0
                );

                GameObject cell = Instantiate(cellPrefab, isoPos, Quaternion.identity);
                cell.transform.localScale = new Vector3(gridInfo.cellSize, gridInfo.cellSize, gridInfo.cellSize);
                cell.transform.parent = this.transform;
                var text = cell.GetComponentInChildren<TextMeshPro>();

                var tile = cell.GetComponent<Tile>();
                tile.tileInfo.id = new Vector2Int(x, y);
                text.text = $"{tile.tileInfo.id}";
                tiles.Add(cell);
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
}
