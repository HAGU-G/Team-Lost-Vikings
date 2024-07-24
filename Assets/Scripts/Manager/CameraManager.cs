using UnityEditorInternal;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private float moveSpeed = 100f;
    private float zoomSpeed = 4f; 
    private float minFov = 15f;
    private float maxFov = 90f;

    private Vector2? lastPanPosition;
    private int fingerId;
    private bool wasZoomingLastFrame;
    private Vector2[] lastZoomPositions;

    private bool IsReady;

    private void Awake()
    {
        GameManager.Subscribe(EVENT_TYPE.LOADED, OnGameLoaded);
    }

    private void OnGameLoaded()
    {
        IsReady = true;
    }

    private void Update()
    {
        if (!IsReady)
            return;

        var im = GameManager.inputManager;

        if(im.Moved && im.receiver.Received && !GameManager.uiManager.isWindowOn)
        {
            transform.position -= im.WorldDeltaPos;
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
}
