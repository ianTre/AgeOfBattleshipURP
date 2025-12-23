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
    public float totalForwardMovementAllowed = 10f;
    public float totalRightMovementAllowed = 10f;

    private Vector3 startPosition;
    private Vector3 movementForward;
    private Vector3 movementRight;
    void Start()
    {
        cam = Camera.main;
        curZoom = cam.transform.localPosition.y;
        curXRot = -50;

        // Initialize movement references
        startPosition = transform.position;

        movementForward = cam.transform.forward;
        movementForward.y = 0.0f;
        movementForward.Normalize();

        movementRight = cam.transform.right;
        movementRight.y = 0.0f;
        movementRight.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isValidStage(GameController.instance.currentStage))
        {
            return;
        }
        if (!DeveloperMode)
            cam.cullingMask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", "Water", "UI");
        else
            cam.cullingMask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", "UI", "Developer");



        /*
        //Rotate
        if (Input.GetMouseButton(1))
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");

            curXRot += -y * rotateSpeed;
            curXRot = Mathf.Clamp(curXRot, minXrot, maxXRot);

            transform.eulerAngles = new Vector3(curXRot, transform.eulerAngles.y + (x * rotateSpeed), 0.0f);
        }
        */

        //Movement
        Vector3 forward = cam.transform.forward;
        forward.y = 0.0f;
        forward.Normalize();

        Vector3 right = cam.transform.right.normalized;


        float moveX = InputController.instance.HorizontalAxisMovement;
        float moveZ = InputController.instance.VerticalAxisMovement;


        Vector3 dir = forward * moveZ + right * moveX;
        if (dir.sqrMagnitude > 0.00001f)
            dir.Normalize();
        dir *= moveSpeed * Time.deltaTime;

        // Desired position after input
        Vector3 desiredPos = transform.position + dir;

        // Compute offset from start along planar basis and clamp it
        Vector3 offset = desiredPos - startPosition;

        float forwardAmount = Vector3.Dot(offset, movementForward);
        float rightAmount = Vector3.Dot(offset, movementRight);

        float clampedForward = Mathf.Clamp(forwardAmount, -totalForwardMovementAllowed, totalForwardMovementAllowed);
        float clampedRight = Mathf.Clamp(rightAmount, -totalRightMovementAllowed, totalRightMovementAllowed);

        Vector3 clampedOffset = movementForward * clampedForward + movementRight * clampedRight;

        Vector3 finalPos = startPosition + clampedOffset;
        // Preserve current Y (movement only in horizontal plane)
        finalPos.y = transform.position.y;

        transform.position = finalPos;

    }

    public void Zoom(float zoomLevel)
    {
        float zoomDirection = zoomLevel>0 ? 1 : -1;
        //ZOOM
        curZoom += zoomDirection * -zoomSpeed;
        curZoom = Mathf.Clamp(curZoom, minZoom, maxZoom);

        cam.transform.localPosition = Vector3.up * curZoom;
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
