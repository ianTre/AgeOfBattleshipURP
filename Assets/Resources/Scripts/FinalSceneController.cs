using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FinalSceneController : MonoBehaviour
{
    float timeStamp;
    float firstFlag = 50f;
    [SerializeField]
    GameObject fire;
    bool endCredits = false;
    AudioSource music;
    // Start is called before the first frame update
    void Start()
    {
        music = GetComponent<AudioSource>();
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
            music.volume = Mathf.Lerp(1f, 0f, timeStamp / 12f);
            fire.GetComponent<AudioSource>().maxDistance = Mathf.Lerp(20f, 50f, timeStamp / 12f);
            yield return null;
        }
        yield return new WaitForSeconds(3f);
        endCredits = true;
    }
}
