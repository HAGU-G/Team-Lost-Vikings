using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileInfo tileInfo;

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

    public void UpdateTileInfo(Tile tile)
    {
        
    }

    private void UpdateTileObject()
    {


        //UpdateTileInfo();
    }
}
