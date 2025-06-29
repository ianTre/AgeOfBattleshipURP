using System.Linq;
using Assets.Resources.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    // Start is called before the first frame update
    AOBLogger logger;
    Camera m_Camera;
    private GameObject hittedObject;
    SelectionController selectionlight;

    void Start()
    {
        logger = new AOBLogger();
        logger.Log("Creating Logger");
        m_Camera = Camera.main;
        selectionlight = FindAnyObjectByType<SelectionController>();
    }

    // Update is called once per frame
    void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse.leftButton.wasPressedThisFrame && GameController.instance.currentStage == GameStage.Deploy)
        {
            SelectShip(mouse);
        }

        if (Input.GetKey(KeyCode.Delete) && GameController.instance.currentStage == GameStage.Deploy)
        {
            if (PlayerController.instance.ships.Any(x => x.hasfocus))
            {
                Ship shipToBeDeleted = PlayerController.instance.ships.Find(x => x.hasfocus);
                shipToBeDeleted.DestroyShip();
            }
        }
    }

    private void SelectShip(Mouse mouse)
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
                    selectionlight.SelectionLightOnSimplified(ship);
                else
                    selectionlight.SelectionLightOff(ship);
            }
        }
    }

}
