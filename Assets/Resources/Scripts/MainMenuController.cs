using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Video;
using Image = UnityEngine.UI.Image;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private Canvas mainMenuCanvas;
    [SerializeField]
    private VideoPlayer videoPlayer;
    [SerializeField]
    private TextMeshProUGUI text1;
    [SerializeField]
    private TextMeshProUGUI text2;
    [SerializeField]
    private TextMeshProUGUI text3;
    [SerializeField]
    private TextMeshProUGUI text4;
    private float videoTime = 8f;
    

    public void Update()
    {
        
    }

    public void StartGame()
    {
        StartCoroutine(LoadNewScene(4));

    }

    private IEnumerator LoadNewScene(float fadeOutTime)
    {
        videoTime -= fadeOutTime;
        Color color1 = text1.color;
        Color color2 = text2.color;
        Color color3 = text3.color;
        Color color4 = text4.color;

        videoPlayer.Play();
        float waiter = 0.5f;
        while (waiter > 0)
        {
            waiter -= Time.deltaTime;
            yield return null;
        }
        mainMenuCanvas.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, 0);
        while (fadeOutTime > 0)
        {
            fadeOutTime -= Time.deltaTime;
            float alpha = fadeOutTime / 3f;
            color1.a = color2.a = color3.a = color4.a = alpha;
            text1.color = color1;
            text2.color = color2;
            text3.color = color3;
            text4.color = color4;
            yield return null;
        }
        while (videoTime > 0)
        {
            videoTime -= Time.deltaTime;
            yield return null;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }

    public void LoadCredits()
    {
        Thread.Sleep(1000);
        UnityEngine.SceneManagement.SceneManager.LoadScene("FinalScene");
    }
}
