using UnityEngine;

[CreateAssetMenu(fileName = "Grid", menuName = "Grid")]
public class GridInfo : ScriptableObject
{
    public Sprite[] useSprites;

    public int row;
    public int col;

    public int minRow;
    public int minCol;
    public float cellSize;

    public SerializableDict<SpriteImageSet> images;

    public Sprite defaultTileSprite;
    public Sprite unusableTileSprite;
}