using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class FinalSceneController : MonoBehaviour
{
    float timeStamp;
    float firstFlag = 50f;
    [SerializeField]
    GameObject fire;
    bool endCredits = false;
    AudioSource music;
    private float startValue;
    [SerializeField]
    GameObject canvasImage;
    Color initialColor;
    // Start is called before the first frame update
    void Start()
    {
        music = GetComponent<AudioSource>();
        startValue = music.volume;
        initialColor = canvasImage.GetComponent<RawImage>().color;
        StartCoroutine(LowerMusic());
    }

    // Update is called once per frame
    void Update()
    {
        if(endCredits)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }

    private IEnumerator LowerMusic()
    {
        yield return new WaitForSeconds(50f);
        while (timeStamp < 12)
        {
            timeStamp += Time.deltaTime;
            music.volume = Mathf.Lerp(startValue, 0f, timeStamp / 12f);
            fire.GetComponent<AudioSource>().maxDistance = Mathf.Lerp(20f, 50f, timeStamp / 12f);
            if(timeStamp > 9)
            {
                canvasImage.GetComponent<RawImage>().color = new Color(initialColor.r, initialColor.g, initialColor.b, Mathf.Lerp(0f, 1f, (timeStamp - 9f) / 3f));
            }
            yield return null;
        }
        yield return new WaitForSeconds(3f);
        endCredits = true;
    }
}
