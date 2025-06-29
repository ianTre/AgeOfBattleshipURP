using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    public float rotateSpeed = 5.0f; // Speed of rotation
    public bool isRotating = false; // Flag to control rotation

    void Update()
    {
        if(isRotating)
            transform.Rotate(0, rotateSpeed * Time.deltaTime, 0); // Rotate around the Y-axis
    }

    public void StartRotation(Vector3 position)
    {
        transform.position = position; // Set the camera position
        transform.rotation = Quaternion.Euler(0, 0, 0); // Reset rotation to default
        isRotating = true; // Start rotating
    }

    public void StopRotation()
    {
        isRotating = false; // Stop rotating
    }
}
