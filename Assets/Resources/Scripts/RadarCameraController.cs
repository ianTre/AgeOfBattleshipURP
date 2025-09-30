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
    public float curZoom;
    public float zoomSpeed;
    [SerializeField]
    private GameObject radarObject;

    [SerializeField]
    public Camera cam;
    private Vector3 deltaMovement;
    [SerializeField]
    private float VerticalMaxMovement;
    [SerializeField]
    private float HorizontalMaxMovement;
    private float originalVerticalMaxMovement;
    private float originalHorizontalMaxMovement;
    void Start()
    {
        cam.orthographicSize = curZoom;
        curXRot = -50;
        deltaMovement = new Vector3(0, 0, 0);
        originalVerticalMaxMovement = VerticalMaxMovement;
        originalHorizontalMaxMovement = HorizontalMaxMovement;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.instance.currentStage != GameStage.PlayerAttackEnemyMap)
        {
            return;
        }
        //PREGUNTAR QUE ES EL DEVELOPER MODE - DESACTIVADO FUNCIONA EL GRID
        /* if (!DeveloperMode)
             cam.cullingMask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", "Water", "UI"); 
             cam.cullingMask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", "UI");*/
        //PREGUNTAR QUE ES EL DEVELOPER MODE

        
        //Movement
        Vector3 forward = new Vector3(0, 0, 1);
        forward.y = 0.0f;
        forward.Normalize();

        Vector3 right = cam.transform.right.normalized;

        float moveX = InputController.instance.HorizontalAxisMovement;
        float moveZ = InputController.instance.VerticalAxisMovement;

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

    public void Zoom(float zoomLevel)
    {
        float zoomDirection = zoomLevel > 0 ? 1 : -1;
        //ZOOM
        curZoom += zoomDirection * -zoomSpeed;
        curZoom = Mathf.Clamp(curZoom, minZoom, maxZoom);

        cam.orthographicSize = curZoom;
        VerticalMaxMovement = curZoom >= 200 ? (float)-0.1 * curXRot + 20 : (float)-0.2 * curXRot + 40;
    }
}
