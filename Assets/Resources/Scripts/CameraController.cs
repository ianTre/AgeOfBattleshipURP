using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    public bool DeveloperMode = false;
    public float moveSpeed;

    public float minXrot;
    public float maxXRot;
    public float curXRot;

    public float minZoom;
    public float maxZoom;
    private float curZoom;
    public float zoomSpeed;
    public float rotateSpeed;
    private Camera cam;
    void Start()
    {
        cam = Camera.main;
        curZoom = cam.transform.localPosition.y;
        curXRot = -50;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isValidStage(GameController.instance.currentStage))
        {
            return;
        }
        if(!DeveloperMode)
            cam.cullingMask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", "Water", "UI");
        else
            cam.cullingMask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", "UI","Developer");

        //ZOOM
        curZoom += Input.GetAxis("Mouse ScrollWheel") * -zoomSpeed;
        curZoom = Mathf.Clamp(curZoom, minZoom, maxZoom);

        cam.transform.localPosition = Vector3.up * curZoom;


        //Rotate
        if (Input.GetMouseButton(1))
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");

            curXRot += -y * rotateSpeed;
            curXRot = Mathf.Clamp(curXRot, minXrot, maxXRot);

            transform.eulerAngles = new Vector3(curXRot, transform.eulerAngles.y + (x * rotateSpeed), 0.0f);
        }

        //Movement
        Vector3 forward = cam.transform.forward;
        forward.y = 0.0f;
        forward.Normalize();

        Vector3 right = cam.transform.right.normalized;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 dir = forward * moveZ + right * moveX;
        dir.Normalize();
        dir *= moveSpeed * Time.deltaTime;
        
        transform.position += dir;

    }

    private bool isValidStage(GameStage currentStage)
    {
        switch (currentStage)
        {
            case GameStage.Deploy :
            case GameStage.PlayerAttackCinematic : 
            case GameStage.IAAttackPlayerMap:
                return true;

            case GameStage.PlayerAttackEnemyMap:
            default :
                return false;
        }
    }
}
