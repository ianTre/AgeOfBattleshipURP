using System.Collections;
using System.Collections.Generic;
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
    public void StartGame()
    {
        StartCoroutine(LoadNewScene(4));
        
    }

    private IEnumerator LoadNewScene(float fadeOutTime)
    {
        videoTime -= fadeOutTime;
        Color currentColor = text1.color;
        videoPlayer.Play();
        float waiter = 0.1f;
        while (waiter > 0 )
        {
            waiter -= Time.deltaTime;
            yield return null;
        }
        mainMenuCanvas.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, 0);
        while (fadeOutTime > 0)
        {
            fadeOutTime -= Time.deltaTime;
            float alpha = fadeOutTime / 3f;
            currentColor.a = alpha;
            text1.color = currentColor;
            text2.color = currentColor;
            text3.color = currentColor;
            text4.color = currentColor;
            yield return null;
        }
        while (videoTime > 0)
        {
            videoTime -= Time.deltaTime;
            yield return null;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }
}
