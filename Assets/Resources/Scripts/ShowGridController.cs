using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowGridController : MonoBehaviour
{
    InputController inputController;
    RadarCameraController radarCameraController;
    // Start is called before the first frame update
    void Start()
    {
        inputController = FindAnyObjectByType<InputController>();
        radarCameraController = FindAnyObjectByType<RadarCameraController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowGridOnEnemyMap()
    {
        inputController.ShowGridOnEnemyMap();
        GridButtonLightController();
    }
    public void GridButtonLightController()
    { 
      //POSIBLE LUGAR PARA EL CAMBIO DE COLOR DEL BOTON
    }
}
