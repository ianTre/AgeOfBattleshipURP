using System;
using System.Linq;
using Assets.Resources.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    // Start is called before the first frame update
    AOBLogger logger;
    Camera m_Camera;
    private GameObject hittedObject;
    SelectionController selectionlight;
    public static InputController instance;
    public bool isMouseDown = false;
    public RadarCameraController radarCameraController;
    private bool isGridOn = false;

    void Start()
    {
        logger = new AOBLogger();
        logger.Log("Creating Logger");
        m_Camera = Camera.main;
        selectionlight = FindAnyObjectByType<SelectionController>();
        radarCameraController = FindAnyObjectByType<RadarCameraController>();
    }

    void Awake()
    {
        instance = this;
    }

    public void UpdateCameraReference()
    {
        m_Camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
        Mouse mouse = Mouse.current;
        if (mouse.leftButton.wasPressedThisFrame)
        {
            isMouseDown = true;
            Debug.Log("Mouse down");
            MouseRayCastSelectionMode(mouse);
        }

        if (mouse.leftButton.wasReleasedThisFrame)
        {
            isMouseDown = false;
        }

        if (Input.GetKey(KeyCode.Delete) && GameController.instance.currentStage == GameStage.Deploy)
        {
            Ship selectedShip = PlayerController.instance.GetSelectedShip();
            if (selectedShip != null)
            {
                PlayerController.instance.ClearSelectedShip();
                selectedShip.DestroyShip();
            }
        }

        if (Input.GetKey(KeyCode.Alpha0)) //REMOVE BEFORE RELEASE
        {
            Ship shipToSunk;
            if ((shipToSunk = PlayerController.instance.GetSelectedShip()) != null)
            {
                shipToSunk.SunkingCinematick();
            }
        }

        if (Input.GetKeyDown(KeyCode.G) && GameController.instance.currentStage == GameStage.PlayerAttackEnemyMap) // muestra el grid
        {
            ShowGridOnEnemyMap();

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

    private void MouseRayCastSelectionMode(Mouse mouse)
    {
        Vector3 mousePosition = mouse.position.ReadValue();
        Ray ray = m_Camera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
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
}
