using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConstructBuilding : MonoBehaviour
{
    public bool isSelected = false;
    public bool isRemoveTime = false;

    public GameObject PlaceBuilding(GameObject obj, Tile tile, GridMap gridMap)
    {
        var objInfo = obj.GetComponent<Building>();
        var width = objInfo.Width;
        var height = objInfo.Length;

        objInfo.placedTiles.Clear();
        var tileId = tile.tileInfo.id;
        var indexX = tileId.x + width - 1;
        var indexY = tileId.y + height - 1;

        var entranceX = tile.tileInfo.id.x - 1;
        var entranceY = tile.tileInfo.id.y;

        if (indexX < 0 || indexY < 0
            || indexX > gridMap.gridInfo.row - 1 || indexY > gridMap.gridInfo.col - 1
            || entranceX < 0 || entranceY < 0
            || entranceX > gridMap.gridInfo.row - 1 || entranceY > gridMap.gridInfo.col - 1)
        {
            Debug.Log("건물을 설치할 수 없습니다.");
            isSelected = false;
            return null;
        }

        objInfo.entranceTile = gridMap.tiles[new Vector2Int(tile.tileInfo.id.x - 1, tile.tileInfo.id.y)];
        var instancedObj = Instantiate(obj, gridMap.IndexToPos(tileId), Quaternion.identity, tile.transform);
        var pos = instancedObj.transform.position;
        pos.y = instancedObj.transform.position.y - gridMap.gridInfo.cellSize / 4f;
        instancedObj.transform.position = pos;
        //construectedBuildings.Add(instancedObj);

        var buildingComponent = instancedObj.GetComponent<Building>();
        buildingComponent.placedTiles.Add(tile);

        for (int i = tileId.x; i <= indexX; ++i)
        {
            for (int j = tileId.y; j <= indexY; ++j)
            {
                var t = gridMap.tiles.GetValueOrDefault(new Vector2Int(i, j));
                t.UpdateTileInfo(TileType.OBJECT, instancedObj);

                objInfo.placedTiles.Add(t);
            }
        }
        isSelected = false;

        return instancedObj;
    }

    public GameObject RemoveBuilding(Tile tile, GridMap gridMap)
    {
        var obj = tile.tileInfo.ObjectLayer.LayerObject;
        var objInfo = obj.GetComponent<Building>();
        var width = objInfo.Width;
        var height = objInfo.Length;

        if (objInfo.placedTiles == null || !objInfo.placedTiles.Any())
            return null;

        var standardTile = objInfo.placedTiles.First();
        var indexX = standardTile.tileInfo.id.x + width - 1;
        var indexY = standardTile.tileInfo.id.y + height - 1;
        for (int i = standardTile.tileInfo.id.x; i <= indexX; ++i)
        {
            for (int j = standardTile.tileInfo.id.y; j <= indexY; ++j)
            {
                var t = gridMap.tiles.GetValueOrDefault(new Vector2Int(i, j));
                t?.ResetTileInfo();

            }
        }

        objInfo.placedTiles.Clear();
        //construectedBuildings.Remove(obj);
        Destroy(obj);
        tile.ResetTileInfo();
        isRemoveTime = false;

        return obj;
    }
}
