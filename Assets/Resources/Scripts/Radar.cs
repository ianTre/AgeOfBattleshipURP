using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    [SerializeField]
    Transform radarSweep;
    private float rotationSpeed = 180f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        radarSweep.eulerAngles += new Vector3(0,0,rotationSpeed * Time.deltaTime);
    }
}
