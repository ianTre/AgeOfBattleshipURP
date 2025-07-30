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

    void Start()
    {
        logger = new AOBLogger();
        logger.Log("Creating Logger");
        m_Camera = Camera.main;
        selectionlight = FindAnyObjectByType<SelectionController>();
    }

    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse.leftButton.wasPressedThisFrame)
        {
            isMouseDown = true;
            Debug.Log("Mouse down");
            if (GameController.instance.currentStage == GameStage.Deploy)
            {
                MouseRayCastSelectionMode(mouse);
            }
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
 
    }

    private void MouseRayCastSelectionMode(Mouse mouse)
    {
        Vector3 mousePosition = mouse.position.ReadValue();
        Ray ray = m_Camera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hittedObject = hit.collider.gameObject;
            // Use the hit variable to determine what was clicked on.
            if (hittedObject.tag == "Ship" || hittedObject.tag == "ShipComponenet")
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
            else
            { 
                PlayerController.instance.ClearSelectedShip();
            }
         
        }
    }

}
