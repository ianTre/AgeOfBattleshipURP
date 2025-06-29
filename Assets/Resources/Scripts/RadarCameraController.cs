using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarCameraController : MonoBehaviour
{
    // Start is called before the first frame update
    public bool DeveloperMode = false;
    public float moveSpeed;
    public float curXRot;

    public float minZoom;
    public float maxZoom;
    private float curZoom;
    public float zoomSpeed;
    [SerializeField]
    private GameObject radarObject;

    [SerializeField]
    private Camera cam;
    private Vector3 deltaMovement;
    [SerializeField]
    private float VerticalMaxMovement;
    [SerializeField]
    private float HorizontalMaxMovement;
    void Start()
    {
        curZoom = cam.transform.localPosition.y;
        curXRot = -50;
        deltaMovement = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.instance.currentStage != GameStage.PlayerAttackEnemyMap)
        {
            return;
        }
        if (!DeveloperMode)
            cam.cullingMask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", "Water", "UI");
        else
            cam.cullingMask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", "UI");

        //ZOOM
        curZoom += Input.GetAxis("Mouse ScrollWheel") * -zoomSpeed;
        curZoom = Mathf.Clamp(curZoom, minZoom, maxZoom);

        cam.transform.localPosition = Vector3.up * curZoom;

        //Movement
        Vector3 forward = new Vector3(0, 0, 1);
        forward.y = 0.0f;
        forward.Normalize();

        Vector3 right = cam.transform.right.normalized;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs((deltaMovement + (right*moveX)).x ) > HorizontalMaxMovement ) 
        {
            moveX = 0;
        }

        if (Mathf.Abs(deltaMovement.z + (forward * moveZ).z) > VerticalMaxMovement)
        {
            moveZ = 0;
        }

        Vector3 dir = forward * moveZ + right * moveX;
        
        dir.Normalize();
        dir *= moveSpeed * Time.deltaTime;
        deltaMovement += dir;
        transform.position += dir;
        radarObject.transform.position += dir;

    }
}
