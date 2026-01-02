using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriCoding : MonoBehaviour
{
    // Start is called before the first frame update
    float timestamp=0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timestamp += Time.deltaTime;
        if (timestamp > 10)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
