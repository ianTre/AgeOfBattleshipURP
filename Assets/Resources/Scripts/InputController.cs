using System;
using System.Collections;
using System.Linq;
using Assets.Resources.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    // Start is called before the first frame update
    Camera m_Camera;
    private GameObject hittedObject;
    SelectionController selectionlight;
    public static InputController instance;
    public bool isTappingOrClicking = false;
    public RadarCameraController radarCameraController;
    private bool isGridOn = false;
    private PlayerInputActions playerActions;
    public float mouseScrollY;
    public Vector2 Position;
    public float VerticalAxisMovement;
    public float HorizontalAxisMovement;
    private Coroutine zoomCoroutine;
    public LayerMask IgnoreMe;

    void Start()
    {
        m_Camera = Camera.main;
        selectionlight = FindAnyObjectByType<SelectionController>();
        radarCameraController = FindAnyObjectByType<RadarCameraController>();
    }

    void Awake()
    {
        instance = this;
        playerActions = new PlayerInputActions();
        playerActions.Player.Zoom.performed += x => mouseScrollY = x.ReadValue<float>();
        playerActions.Player.Position.performed += x => Position = x.ReadValue<Vector2>();
        playerActions.Player.VerticalAxis.performed += x => VerticalAxisMovement = x.ReadValue<float>();
        playerActions.Player.VerticalAxis.canceled += x => VerticalAxisMovement = 0;
        playerActions.Player.HorizontalAxis.performed += x => HorizontalAxisMovement = x.ReadValue<float>();
        playerActions.Player.HorizontalAxis.canceled += x => HorizontalAxisMovement = 0;
        playerActions.Player.DeleteAction.performed += x => Delete();
        playerActions.Player.Selection.performed += x => isTappingOrClicking = true;
        //playerActions.Player.Selection.canceled += x => isTappingOrClicking = false;
        playerActions.Player.SecondaryTouchConntact.started += _ => ZoomStart();
        playerActions.Player.SecondaryTouchConntact.canceled += _ => ZoomEnds();
    }

    private void ZoomEnds()
    {
        Debug.Log("Ending ZOOM");
        StopCoroutine(zoomCoroutine);
    }

    private void ZoomStart()
    {
        zoomCoroutine = StartCoroutine(ZoomDetection());
    }

    IEnumerator ZoomDetection()
    {
        float previousDistance = 0f ;
        while(true)
        {
            float distance = Vector2.Distance
                (playerActions.Player.PrimaryFingerPosition.ReadValue<Vector2>()
                , playerActions.Player.SecondaryFingerPosition.ReadValue<Vector2>());

            if (distance > previousDistance )
            {
                CameraManager.instance.HandleZoom(1);
            }

            if (distance < previousDistance)
            {
                CameraManager.instance.HandleZoom(-1);
            }

            previousDistance = distance;
            yield return null;
        }
    }

    public void UpdateCameraReference()
    {
        m_Camera = Camera.main;
    }

    private void OnEnable()
    {
        playerActions.Enable();
    }

    private void OnDisable()
    {
        playerActions.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        //Handle Camera Zoom
        if(mouseScrollY != 0)
        {
            CameraManager.instance.HandleZoom(mouseScrollY);
        }

        if (isTappingOrClicking)
        {
            MouseRayCastSelectionMode();
            isTappingOrClicking = false;
        }

    }

    private static void Delete()
    {
        if(GameController.instance.currentStage != GameStage.Deploy)
        {
            return;
        }
        Ship selectedShip = PlayerController.instance.GetSelectedShip();
        if (selectedShip != null)
        {
            PlayerController.instance.ClearSelectedShip();
            selectedShip.DestroyShip();
        }
    }

    public void ShowGridOnEnemyMap()
    {
        if (!isGridOn)
        {
            radarCameraController.cam.cullingMask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", "Water", "UI");
            isGridOn = true;
        }
        else
        {
            radarCameraController.cam.cullingMask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", "UI");
            isGridOn = false;
        }
    }

    private void MouseRayCastSelectionMode()
    {
//        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = m_Camera.ScreenPointToRay(Position);
        if (Physics.Raycast(ray, out RaycastHit hit,1000f, ~IgnoreMe))
        {
            hittedObject = hit.collider.gameObject;
            // Use the hit variable to determine what was clicked on.
            if (GameController.instance.currentStage == GameStage.Deploy && (hittedObject.tag == "Ship" || hittedObject.tag == "ShipComponenet"))
            {
                ClickOnShip();
                return;
            }
            if (hittedObject.tag == "WaterTile")
            {
                ClickOnWaterTile();
                return;
            }
            ClearAllSelections();
            

        }
    }

    private void ClickOnShip()
    {
        while (hittedObject.tag != "Ship")
        {
            hittedObject = hittedObject.transform.parent.gameObject;
        }
        Ship ship = hittedObject.GetComponent<Ship>();

        if (!ship.hasfocus)
            PlayerController.instance.SetSelectedShip(ship);
        else
            PlayerController.instance.ClearSelectedShip();
    }

    static void ClearAllSelections()
    {
        PlayerController.instance.ClearSelectedShip();
        EnemyMapController.instance.ClearTileSelection();
    }
    private void ClickOnWaterTile()
    {
        Tile selectedTile = hittedObject.GetComponent<Tile>();
        if (!selectedTile.isEnemyTile)
            return;
        selectedTile.TileBeingClicked();
    }

    internal void DraggingIcon()
    {
        VerticalAxisMovement = HorizontalAxisMovement = 0;
    }
}
