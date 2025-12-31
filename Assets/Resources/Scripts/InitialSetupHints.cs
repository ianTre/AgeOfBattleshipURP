using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class InitialSetupHints : MonoBehaviour
{
    // Start is called before the first frame update
    float timeStamp = 5;
    [SerializeField]
    RawImage image;

    [SerializeField]
    TMP_Text m_TextMeshPro;
    int lastValue = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeStamp -= Time.deltaTime;
        if ( timeStamp <= 0)
        {
            ShowHint();
            timeStamp = 15f;
        }
        if( PlayerController.instance.ships.Count != lastValue)
        {
            lastValue = PlayerController.instance.ships.Count;
            timeStamp = 30f;
        }


    }

    private void ShowHint()
    {
        image.gameObject.SetActive(true);
        m_TextMeshPro.gameObject.SetActive(true);
        image.GetComponent<PlayableDirector>().Play();
        Invoke("hideHint", 6f);
    }

    private void hideHint()
    {
        image.GetComponent<PlayableDirector>().Stop();
        image.gameObject.SetActive(false);
        m_TextMeshPro.gameObject.SetActive(false);
    }
}
