using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIssileController : MonoBehaviour
{
    [SerializeField]
    Vector3 rotationSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
