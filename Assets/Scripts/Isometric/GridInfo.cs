﻿using UnityEngine;

[CreateAssetMenu(fileName = "Grid", menuName = "Grid")]
public class GridInfo : ScriptableObject
{
    public int row;
    public int col;

    public int minRow;
    public int minCol;
    public float cellSize;
}
