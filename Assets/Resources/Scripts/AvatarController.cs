using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class AvatarController : MonoBehaviour
{
    [SerializeField]
    TMP_Text m_TextMeshPro;
    
    PlayableDirector PlayableDirector;
    OldConsoleText oldConsoleText;
    [SerializeField]
    GameObject avatarPanel;
    private bool coroutineActive = false;
    // Start is called before the first frame update
    void Awake()
    {
        PlayableDirector = GetComponent<PlayableDirector>();
        oldConsoleText = m_TextMeshPro.GetComponent<OldConsoleText>();
    }

    public void DisplayWelcomeMessage()
    {
        EnemyMapController.instance.firingButtonEnabled = false;
        avatarPanel.SetActive(true);
        PlayableDirector.Play();
        List<string> messages = new List<string>()
        {
            "Hello there captain , my name is lieutenant Dan, and i will help you in this battle ",
            "What you are seeing now is a simulation of the enemy battlefield in your radar ",
            "Your mission is to destroy all enemy targets before they destroy you ",
            "First you need to select an Enemy Spot in the radar ",
            "Then press Fire button on the bottom right corner "
        };
        StartCoroutine(ShowMessageCoroutine(messages));
        coroutineActive=true;
    }

    public IEnumerator ShowMessageCoroutine(List<string> messages)
    {
        yield return new WaitForSeconds(1.7f);
        foreach (string message in messages)
        {
            oldConsoleText.StartDisplayingText(message);
            while (oldConsoleText.isDisplayingText)
            {
                /*if (playerWantToSkip)
                    oldConsoleText.interruptConsole();*/
                yield return null;
            }
            yield return new WaitForSeconds(3f);
        }
        yield return new WaitForSeconds(2f);
        coroutineActive = false;
        avatarPanel.SetActive(false);
        EnemyMapController.instance.firingButtonEnabled = true;
    }


}
