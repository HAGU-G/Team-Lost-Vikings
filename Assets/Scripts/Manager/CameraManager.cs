using System;
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
    public float prevZoom;
    public bool isHideUnits = false;

    public Vector3 DeltaPos { get; private set; }

    public bool isFocusOnUnit = false;
    public UnitStats focusingUnit;

    public event Action OnLocationChanged;

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
        if (!IsReady || isFocusOnUnit)
            return;

        var im = GameManager.inputManager;

        if (im.Moved
            && im.receiver.Received
            && (!GameManager.uiManager.IsWindowOn || GameManager.villageManager.constructMode.isConstructMode))
        {
            SetPosition(transform.position - im.WorldCenterDeltaPos);
        }

        if (im.Zoom != 0f
            && im.receiver.Received
            && !GameManager.uiManager.IsWindowOn)
        {
            Zoom(ZoomValue - im.Zoom * zoomMagnification);
        }
    }

    private void LateUpdate()
    {
        if (isFocusOnUnit)
            SetPositionOnUnit();

        DeltaPos = Vector3.zero;
    }

    public void Zoom(float value)
    {
        ZoomValue = value;
        Camera.main.orthographicSize = ZoomValue;
    }

    public void SetLocation(LOCATION location, int huntzoneNum = -1, bool isFinishFocusing = true)
    {
        if (isFinishFocusing)
            FinishFocousOnUnit();

        LookLocation = location;

        var sm = GameManager.soundManager;
        if (sm != null)
            sm.SetLocation(location, huntzoneNum);

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

        OnLocationChanged?.Invoke();
    }

    public void SetPosition(Vector3 pos)
    {
        if (gridMap == null || !IsReady)
            return;

        if (gridMap.PosToIndex(pos) != Vector2Int.one * -1)
        {
            var position = pos;
            position.z = -10;
            DeltaPos += position - transform.position;
            transform.position = position;
        }
    }

    private void SetPositionOnUnit()
    {
        if (focusingUnit == null || focusingUnit.isDead)
        {
            FinishFocousOnUnit();
            return;
        }

        Zoom(minZoom);
        var position = focusingUnit.objectTransform.position;
        position.z = -10;
        SetLocation(focusingUnit.Location, focusingUnit.HuntZoneNum, false);
        SetPosition(position);
    }

    public void StartFocusOnUnit(UnitStats stats)
    {
        if (stats == focusingUnit && isFocusOnUnit)
            return;

        if (!isFocusOnUnit)
            prevZoom = ZoomValue;

        if (stats != null)
            focusingUnit = stats;

        isFocusOnUnit = true;
    }

    public void FinishFocousOnUnit()
    {
        if (isFocusOnUnit)
            ZoomValue = prevZoom;
        Camera.main.orthographicSize = ZoomValue;
        isFocusOnUnit = false;

        if (GameManager.IsReady)
            GameManager.uiManager.windows[WINDOW_NAME.TOUCH_UNIT_BUTTONS].Close();
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
