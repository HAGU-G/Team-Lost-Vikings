﻿using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private float moveSpeed = 100f;
    private float zoomSpeed = 4f; 
    private float minFov = 15f;
    private float maxFov = 90f;

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, vertical, 0);
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

        float fov = Camera.main.fieldOfView;
        fov -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        Camera.main.fieldOfView = fov;



    }
}
