using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;

public class VillageManager : MonoBehaviour
{
    public List<GameObject> construectedBuildings = new List<GameObject>();
    public GridMap gridMap;
    private bool isSelected = false;
    private bool isRemoveTime = false;
    public Dictionary<string, GameObject> objectList = new Dictionary<string, GameObject>();
    public GameObject hospital;

    private GameObject selectedObj;

    private void Awake()
    {
        GameStarter.Instance.SetActiveOnComplete(gameObject);
    }

    private void Start()
    {
        objectList.Add("Hospital", hospital);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0f, 0f, 70f, 70f), "Hospital"))
        {
            selectedObj = objectList.GetValueOrDefault("Hospital");
            isSelected = true;
        }

        if (GUI.Button(new Rect(0f, 80f, 70f, 70f), "Remove"))
        {
            isRemoveTime = true;
        }
    }

    public void PlaceObject(GameObject obj, Tile tile)
    {
        var objInfo = obj.GetComponent<Building>();
        var width = objInfo.Width;
        var height = objInfo.Length;

        objInfo.placedTiles.Clear();
        var tileId = tile.tileInfo.id;
        var indexX = tileId.x + width - 1;
        var indexY = tileId.y + height - 1;

        if (indexX < 0 || indexY < 0 || indexX > gridMap.gridInfo.row || indexY > gridMap.gridInfo.col)
        {
            Debug.Log("건물을 설치할 수 없습니다.");
            isSelected = false;
            return;
        }

        var instancedObj = Instantiate(obj, gridMap.IndexToPos(tileId), Quaternion.identity, tile.transform);
        var pos = instancedObj.transform.position; 
        pos.y = instancedObj.transform.position.y - gridMap.gridInfo.cellSize / 4f;
        instancedObj.transform.position = pos;
        construectedBuildings.Add(instancedObj);
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
    }

    public void RemoveObject(Tile tile)
    {
        var obj = tile.tileInfo.ObjectLayer.LayerObject;
        var objInfo = obj.GetComponent<Building>();
        var width = objInfo.Width;
        var height = objInfo.Length;

        if (objInfo.placedTiles == null || !objInfo.placedTiles.Any()) 
            return;

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
        construectedBuildings.Remove(obj);
        Destroy(obj);
        tile.ResetTileInfo();
        isRemoveTime = false;
    }

    private void InteractObject()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 100f);

        if (hit.collider != null)
        {
            Building building = hit.transform.gameObject.GetComponent<Building>();
            if (building != null)
            {
                building.Interact();
            }
        }
    }

    public void PlaceRoad(Tile tile)
    {
        //tile.tileInfo.ObjectLayer = 
        //tile.UpdateTileInfo(TileType.ROAD, tilePrefab);
    }

    public Tile GetTile(Vector3 position)
    {
        var tileId = gridMap.PosToIndex(position);
        if (tileId.x < 0 || tileId.y < 0)
            return null;
        return gridMap.tiles[tileId];
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && isSelected)
        {
            var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var tile = GetTile(worldPos);
            PlaceObject(selectedObj, tile);
        }

        if (Input.GetMouseButtonDown(0) && !isSelected)
        {
            InteractObject();
        }

        if (Input.GetMouseButtonDown(0))
        {
            var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var tile = GetTile(worldPos);
            Debug.Log(tile.tileInfo.TileType.ToString());
        }

        if (Input.GetMouseButtonDown(0) && isRemoveTime)
        {
            var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var tile = GetTile(worldPos);
            RemoveObject(tile);
        }

        //if (Input.GetMouseButtonDown(0))
        //{
        //    var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    if (GetTile(worldPos) != null)
        //    {
        //        var tile = GetTile(worldPos);
        //        Debug.Log($"autoTileId : {tile.tileInfo.autoTileId}");
        //    }
        //}
    }
}
