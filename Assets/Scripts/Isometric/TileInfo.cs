using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer
{
    public GameObject LayerObject { get; set; }
    public bool IsEmpty { get; set; } = true;
}

public enum TileType
{
    NONE,
    ROAD,
    OBJECT
}

public class TileInfo
{
    public Vector2Int id;
    public int Weight {  get; set; }
    public Layer BaseLayer { get; set; }
    public Layer RoadLayer { get; set; }
    public Layer ObjectLayer { get; set; }

    public TileType TileType { get; set; } = TileType.NONE;
   

}
