using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private bool IsReady;
    private GridMap gridMap;
    public LOCATION LookLocation { get; private set; }
    public int HuntZoneNum { get; private set; }

    public float minZoom = 5f;
    public float maxZoom = 30f;
    public float zoomMagnification = 0.05f;
    private float _zoomValue;

    public bool isFocousOnUnit = false;
    public Unit unit;

    public float ZoomValue
    {
        get
        {
            return _zoomValue;
        }
        set
        {
            _zoomValue = Mathf.Clamp(value, minZoom, maxZoom);
        }
    }

    private void Awake()
    {
        if (GameManager.cameraManager != null)
        {
            Destroy(gameObject);
            return;
        }
        GameManager.cameraManager = this;
        GameManager.Subscribe(EVENT_TYPE.CONFIGURE, OnGameStart);
        ZoomValue = Camera.main.orthographicSize;
    }

    private void OnGameStart()
    {
        IsReady = true; 
        SetLocation(LOCATION.VILLAGE);
    }

    private void Update()
    {
        if (!IsReady)
            return;

        if(isFocousOnUnit)
        {
            SetPositionOnUnit(unit.transform.position);
            return;
        }

        var im = GameManager.inputManager;

        if (im.Moved
            && im.receiver.Received
            && (!GameManager.uiManager.isWindowOn || GameManager.villageManager.constructMode.isConstructMode))
        {
            SetPosition(transform.position - im.WorldDeltaPos);
        }

        if (!GameManager.uiManager.isWindowOn)
        {
            ZoomValue -= im.Zoom * zoomMagnification;
            Camera.main.orthographicSize = ZoomValue;
        }
    }


    public void SetLocation(LOCATION location, int huntzoneNum = -1)
    {
        LookLocation = location;
        switch (location)
        {
            case LOCATION.NONE:
                break;
            case LOCATION.VILLAGE:
                gridMap = GameManager.villageManager.gridMap;

                //TO-DO : SetPosition 테스트 후 되돌리기
                //SetPosition(gridMap.gameObject.transform.position);
                foreach (var constructed in GameManager.villageManager.constructedBuildings)
                {
                    var building = constructed.GetComponent<Building>();
                    if (building.StructureType == STRUCTURE_TYPE.STANDARD)
                    {
                        SetPosition(building.transform.position);
                        break;
                    }
                }
                break;
            case LOCATION.HUNTZONE:
                var huntZones = GameManager.huntZoneManager.HuntZones;
                if (huntZones.ContainsKey(huntzoneNum))
                {
                    HuntZoneNum = huntzoneNum;
                    gridMap = huntZones[huntzoneNum].gridMap;
                    SetPosition(huntZones[huntzoneNum].transform.position);
                }
                break;
        }
    }

    public void SetPosition(Vector3 pos)
    {
        if (gridMap == null || !IsReady)
            return;

        if (gridMap.PosToIndex(pos) != Vector2Int.one * -1)
        {
            var position = pos;
            position.z = -10;
            transform.position = position;
        }
    }

    public void SetPositionOnUnit(Vector3 pos)
    {
        ZoomValue = maxZoom * zoomMagnification;
        Camera.main.orthographicSize = ZoomValue;
        var position = pos;
        position.z = -10;
        transform.position = position;
    }

    public void FinishFocousOnUnit()
    {
        ZoomValue = 15f;
        Camera.main.orthographicSize = ZoomValue;
        isFocousOnUnit = false;
    }

    public void SetViewPoint(UnitStats unit)
    {
        if (unit == null)
            return;

        switch (unit.Location)
        {
            case LOCATION.NONE:
                SetLocation(LOCATION.VILLAGE);
                break;
            case LOCATION.VILLAGE:
                gridMap = GameManager.villageManager.gridMap;
                foreach (var character in GameManager.villageManager.village.units)
                {
                    if (character.stats == unit)
                    {
                        SetPosition(character.transform.position);
                        break;
                    }
                }
                break;
            case LOCATION.HUNTZONE:
                var huntZones = GameManager.huntZoneManager.HuntZones;
                if (!huntZones.ContainsKey(unit.HuntZoneNum))
                    return;

                gridMap = huntZones[unit.HuntZoneNum].gridMap;
                foreach (var character in GameManager.huntZoneManager.HuntZones[unit.HuntZoneNum].Units)
                {
                    if (character.stats == unit)
                    {
                        SetPosition(character.transform.position);
                        break;
                    }
                }
                break;
        }
    }
}
