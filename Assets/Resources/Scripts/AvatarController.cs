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
    private bool playerWantToSkip = false;
    [SerializeField]
    AudioClip WelcomeMessage1;
    [SerializeField]
    AudioClip WelcomeMessage2;
    [SerializeField]
    AudioClip WelcomeMessage3;
    [SerializeField]
    AudioClip WelcomeMessage4;
    [SerializeField]
    AudioClip WelcomeMessage5;
    [SerializeField]
    AudioClip EnemyShipSunk;
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
        var messages = new List<(string,AudioClip)>()
        {
            ("Hello there captain! My name is lieutenant Dan, and I will help you in this battle ",WelcomeMessage1),
            ("What you are seeing now is a simulation of the enemy battlefield on your radar ",WelcomeMessage2),
            ("Your mission is to destroy all enemy targets before they destroy you ",WelcomeMessage3),
            ("First, you need to select an Enemy Spot on the radar ",WelcomeMessage4),
            ("Then, press the Fire button on the bottom right corner ", WelcomeMessage5)
        };
        StartCoroutine(ShowMessageCoroutine(messages));
        coroutineActive=true;
    }

    public void DisplayPlayerSunkAShip()
    {
        EnemyMapController.instance.firingButtonEnabled = false;
        avatarPanel.SetActive(true);
        PlayableDirector.Play();
        var messages = new List<(string,AudioClip)>()
        {
            ("Congratulations Captain! You have sunk an enemy ship. ", EnemyShipSunk)
        };
        StartCoroutine(ShowMessageCoroutine(messages));
        coroutineActive = true;
    }

    public void DisplayPlayerHitAShip()
    {
        EnemyMapController.instance.firingButtonEnabled = false;
        avatarPanel.SetActive(true);
        PlayableDirector.Play();
        var messages = new List<(string, AudioClip)>()
        {
            ("Enemy ship found.We should focus our fire power in that position. ", null)
        };
        StartCoroutine(ShowMessageCoroutine(messages));
        coroutineActive = true;
    }

    public void Skip()
    {
        playerWantToSkip = true;
        oldConsoleText.interruptConsole();
    }

    public IEnumerator ShowMessageCoroutine(List<(string Text,AudioClip Audio)> messages)
    {
        yield return new WaitForSeconds(1.1f);
        foreach (var message in messages)
        {
            oldConsoleText.StartDisplayingText(message.Text);
            if(message.Audio != null)
            {
                GetComponent<AudioSource>().clip = message.Audio;
                GetComponent<AudioSource>().Play();
            }
            while (oldConsoleText.isDisplayingText)
            {
                yield return null;
            }
            if(playerWantToSkip)
            {
                GetComponent<AudioSource>().Stop();
                playerWantToSkip = false;
                continue;
            }
            yield return new WaitForSeconds(2.0f);
        }
        yield return new WaitForSeconds(1.5f);
        coroutineActive = false;
        avatarPanel.SetActive(false);
        GameController.instance.UpdateStage(GameStage.PlayerAttackCinematicFinished);
        EnemyMapController.instance.firingButtonEnabled = true;
    }


}
