using UnityEngine;

public class CameraManager : MonoBehaviour
{
    //private float moveSpeed = 100f;
    //private float zoomSpeed = 4f;
    //private float minFov = 15f;
    //private float maxFov = 90f;

    private Vector2? lastPanPosition;
    private int fingerId;
    private bool wasZoomingLastFrame;
    private Vector2[] lastZoomPositions;

    private bool IsReady;
    private GridMap gridMap;
    public LOCATION LookLocation { get; private set; }
    public int HuntZoneNum { get; private set; }

    private void Awake()
    {
        if (GameManager.cameraManager != null)
        {
            Destroy(gameObject);
            return;
        }
        GameManager.cameraManager = this;

        GameManager.Subscribe(EVENT_TYPE.START, OnGameStart);
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

        var im = GameManager.inputManager;

        if (im.Moved && im.receiver.Received && !GameManager.uiManager.isWindowOn)
        {
            SetPosition(transform.position - im.WorldDeltaPos);
        }

        if(im.Moved && im.receiver.Received && GameManager.villageManager.constructMode.isConstructMode)
        {
            SetPosition(transform.position - im.WorldDeltaPos);
        }

        //float horizontal = Input.GetAxis("Horizontal");
        //float vertical = Input.GetAxis("Vertical");

        //Vector3 movement = new Vector3(horizontal, vertical, 0);
        //transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

        //float fov = Camera.main.orthographicSize;
        //fov -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        //fov = Mathf.Clamp(fov, minFov, maxFov);
        //Camera.main.orthographicSize = fov;

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
