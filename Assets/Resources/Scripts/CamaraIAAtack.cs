using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraIAAtack : MonoBehaviour
{
    public float deltaXPos;
    public float deltaYPos;
    public float deltaZPos;
    public float deltaXRotation;
    public float deltaYRotation;
    public float deltaZRotation;


    void Update()
    {
    }

    public void StartVisualization(Vector3 position)
    {
        transform.position = new Vector3(position.x + deltaXPos , deltaYPos , position.z + deltaZPos ); // Set the camera position
        transform.rotation = Quaternion.Euler(deltaXRotation, deltaYRotation, deltaZRotation); // Reset rotation to default
    }

}