using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Grid", menuName = "Grid")]
public class GridInfo : ScriptableObject
{
    public int col;
    public int row;
    public float cellSize;
}
