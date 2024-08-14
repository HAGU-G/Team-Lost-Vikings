using System;
using System.Text;
using UnityEngine;

public enum Sides
{
    Bottom,
    Right,
    Left,
    Top,
}

public class Cell : MonoBehaviour
{
    public CellInfo tileInfo;

    public Cell[] adjacentTiles { get; } = new Cell[4];
    public Cell previous = null;
    public GridMap gridMap; //자신이 속한 gridMap

    private void Awake()
    {
        tileInfo = new CellInfo();
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

        tileInfo.MarginLayer = new Layer
        {
            LayerObject = null,
            IsEmpty = true
        };
    }

    public bool CanMove
    {
        get
        {
            return (tileInfo.TileType != TileType.OBJECT);
        }
    }

    public void Clear()
    {
        previous = null;
    }

    public void ResetTileInfo()
    {
        tileInfo.RoadLayer.LayerObject = null;
        tileInfo.RoadLayer.IsEmpty = true;
        tileInfo.ObjectLayer.LayerObject = null;
        tileInfo.ObjectLayer.IsEmpty = true;
        tileInfo.MarginLayer.LayerObject = null;
        tileInfo.MarginLayer.IsEmpty = true;
        tileInfo.TileType = TileType.NONE;

        UpdateAutoTileId();
    }

    public void TileColorChange(bool canBuild)
    {
        if(canBuild)
            GetComponent<SpriteRenderer>().material.color = Color.green;
        else
            GetComponent<SpriteRenderer>().material.color = Color.red;
    }

    public void RestoreTileColor()
    {
        GetComponent<SpriteRenderer>().material.color = Color.white;
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
            case TileType.MARGIN:
                tileInfo.MarginLayer.IsEmpty = false;
                tileInfo.TileType = TileType.MARGIN;
                break;
        }
    }

    //public void ClearTileInfo()
    //{
    //    tileInfo.RoadLayer.LayerObject = null;
    //    tileInfo.RoadLayer.IsEmpty = true;
    //    tileInfo.ObjectLayer.LayerObject = null;
    //    tileInfo.ObjectLayer.IsEmpty = true;
    //    tileInfo.TileType = TileType.NONE;
    //}

    public void UpdateAutoTileId()
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < adjacentTiles.Length; ++i)
        {
            if (adjacentTiles[i] != null)
                sb.Append(
                    (adjacentTiles[i].tileInfo.TileType != TileType.OBJECT) //임시로 모두 길이라고 설정
                    //(adjacentTiles[i].tileInfo.TileType != TileType.NONE)
                    //&&(adjacentTiles[i].tileInfo.TileType != TileType.OBJECT)
                    ? "1" : "0");
        }
        tileInfo.autoTileId = Convert.ToInt32(sb.ToString(), 2); //2진수를 10진수로 변환해서 autoTileId에 할당해줌
    }

    public void RemoveAdjacents(Cell tile)
    {
        for (int i = 0; i < adjacentTiles.Length; ++i)
        {
            if (adjacentTiles[i] == null)
            { continue; }

            if (adjacentTiles[i].tileInfo.id == tile.tileInfo.id)
            {
                adjacentTiles[i] = null; //TO-DO : 수정하기
                break;
            }
        }
        UpdateAutoTileId();
    }

    public void ClearAdjacents() //연결된 타일도 지우고 내 이웃도 지우기
    {
        for (int i = 0; i < adjacentTiles.Length; ++i)
        {
            if (adjacentTiles[i] == null)
            { continue; }

            adjacentTiles[i].RemoveAdjacents(this);
            adjacentTiles[i] = null;
        }
        UpdateAutoTileId();
    }
}
