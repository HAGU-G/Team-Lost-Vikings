using UnityEngine;

public class Layer
{
    public GameObject LayerObject { get; set; }
    public bool IsEmpty { get; set; } = true;
}

public enum TileType
{
    NONE = -1,
    ROAD = 0,
    OBJECT = 15,
}

public class CellInfo
{
    public Vector2Int id;
    public int autoTileId;
    public int Weight {  get; set; }
    public Layer BaseLayer { get; set; }
    public Layer RoadLayer { get; set; }
    public Layer ObjectLayer { get; set; }
    public Texture2D defaultTileTexture { get; set; }
    public TileType TileType { get; set; } = TileType.NONE;
}
