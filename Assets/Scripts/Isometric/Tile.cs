using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileInfo tileInfo;
    [field: SerializeField]
    public List<Tile> AdjacentTiles { get; set; } = new List<Tile>();

    private void Awake()
    {
        tileInfo = new TileInfo();
        tileInfo.Weight = 1;
        tileInfo.BaseLayer = new Layer
        {
            LayerObject = null,
            IsEmpty = true
        };

        tileInfo.RoadLayer = new Layer
        {
            LayerObject = null,
            IsEmpty = true
        };

        tileInfo.ObjectLayer = new Layer
        {
            LayerObject = null,
            IsEmpty = true
        };
    }

    public void ResetTileInfo()
    {
        tileInfo.RoadLayer.LayerObject = null;
        tileInfo.RoadLayer.IsEmpty = true;
        tileInfo.ObjectLayer.LayerObject = null;
        tileInfo.ObjectLayer.IsEmpty = true;
        tileInfo.TileType = TileType.NONE;
    }

    public void UpdateTileInfo(TileType type, GameObject obj)
    {
        switch (type)
        {
            case TileType.ROAD:
                tileInfo.RoadLayer.LayerObject = obj;
                tileInfo.RoadLayer.IsEmpty = false;
                tileInfo.TileType = TileType.ROAD;
                break;
            case TileType.OBJECT:
                tileInfo.ObjectLayer.LayerObject = obj;
                tileInfo.ObjectLayer.IsEmpty = false;
                tileInfo.TileType = TileType.OBJECT;
                break;
        }
    }

    public void ClearTileInfo()
    {
        tileInfo.RoadLayer.LayerObject = null;
        tileInfo.RoadLayer.IsEmpty = true;
        tileInfo.ObjectLayer.LayerObject = null;
        tileInfo.ObjectLayer.IsEmpty = true;
        tileInfo.TileType = TileType.NONE;

        //TO-DO : 인접 타일들이 가지는 정보도 수정해줘야함
    }

    private void UpdateTileObject()
    {


        //UpdateTileInfo();
    }
}
