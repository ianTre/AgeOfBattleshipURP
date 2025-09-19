using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public GameStage currentStage;
    private GameStage actionStage;
    public int turn = 0;
    [SerializeField]
    Camera camera1;
    [SerializeField]
    Camera camera2;
    [SerializeField]
    Camera camera3;
    [SerializeField]
    Camera camera4;
    [SerializeField]
    GameObject endGameCanvas;
    [SerializeField]
    AudioClip gameOverSound;
    public bool FixedIAShot = false; //REMOVE ASAP

    List<string> coordinates = new List<string>();
    private Ship shipToAction;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        currentStage = GameStage.Deploy;
        CameraManager.instance.ChangeToDeployStage();

        coordinates.Add("0,0");
        coordinates.Add("0,1");
        coordinates.Add("0,2");
        coordinates.Add("0,3");
        coordinates.Add("0,4");
        coordinates.Add("0,5");
        coordinates.Add("0,6");
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        //BORRAR ABAJO
        if (Input.GetKeyDown(KeyCode.J))
        {
            EndOfGame("German");
        }
        //BORRAR ARRIBA

        if (actionStage == currentStage)
            return;

        switch (actionStage)
        {
            case GameStage.PlayerAttackEnemyMap:
                StartCoroutine(TransitionToPlayerAttackEnemyMap());
                break;

            case GameStage.PlayerAttackCinematic:
                TransitionToPlayerAttackCinematic();
                break;

            case GameStage.IAAttackPlayerMap:
                TransitionToIAAttack();
                break;

            case GameStage.IAAttackCinematic:
                TransitionToIAAttackCinematic();
                break;
            default:
                break;
        }
    }

    public IEnumerator TransitionToPlayerAttackEnemyMap()
    {
        if (currentStage == GameStage.Deploy)
        {
            EndDeployStage();
        }
        yield return new WaitForSeconds(1);
        GameObject.Find("InitialSetupCanvas")?.SetActive(false);
        currentStage = GameStage.PlayerAttackEnemyMap;
        CameraManager.instance.ChangeToPlayerAtacckIARadar();
        shipToAction = PlayerController.instance.getShipToBeActioned();
        GameObject.Find("CameraRotator").GetComponent<CameraRotator>().StartRotation(shipToAction.transform.position);
        turn++;
    }

    public void TransitionToPlayerAttackCinematic()
    {
        currentStage = GameStage.PlayerAttackCinematic;
        shipToAction.GetComponent<FirePowerController>().FireCannons();
        StartCoroutine(CWaitForSeconds(2.0f));
    }

    IEnumerator CWaitForSeconds(float v)
    {
        while (shipToAction.GetComponent<FirePowerController>().isFiring)
        {
            yield return null;
        }
        yield return new WaitForSeconds(v);
        if (EnemyMapController.instance.CheckEndOfGame())
        {
            actionStage = GameStage.EndOfGame;
            EndOfGame("Player");
        }
        else
        {
            actionStage = GameStage.IAAttackPlayerMap;
        }
        GameObject.Find("CameraRotator").GetComponent<CameraRotator>().StopRotation();
    }

    public void TransitionToIAAttack()
    {
        currentStage = GameStage.IAAttackPlayerMap;
        //when FixedIAShot is removed , delete from here
        if (FixedIAShot)
        {
            var rowNumber = int.Parse(coordinates[0].Split(',')[0]);
            var columnNumber = int.Parse(coordinates[0].Split(',')[1]);
            coordinates.RemoveAt(0);
            PlayerController.instance.ProcessEnemyHit(rowNumber, columnNumber);
        }
        else //Until here included
            EnemyMapController.instance.IAEnemyShot();

        if (PlayerController.instance.CheckEndOfGame())
        {
            actionStage = GameStage.EndOfGame;
            EndOfGame("IA");
        }
        else
            actionStage = GameStage.IAAttackCinematic;
    }

    public void TransitionToIAAttackCinematic()
    {
        currentStage = GameStage.IAAttackCinematic;
        CameraManager.instance.ChangeToPlayerAtacckIACinematick();
        AnimationController.instance.PlayExplosion();
        //Next step needs to be triggered by animation event
        //actionStage = GameStage.PlayerAttackEnemyMap; 
    }

    public void EndDeployStage()
    {
        Debug.Log("End Deploy Stage");
        currentStage = GameStage.PlayerAttackEnemyMap;
        List<Tile> playerTiles = MapController.instance.AllTiles;
        EnemyMapController.instance.GenerateEnemyMap(playerTiles);
        List<Ship> ships = PlayerController.instance.ships;
        PlayerController.instance.ClearSelectedShip();
        EnemyMapController.instance.GenerateEnemyShips(ships);
    }

    public void EndOfGame(string winner)
    {
        GameObject endGamePanel;
        AudioSource audio;

        if (winner == "IA") // In case we want different end game for IA
        {
            currentStage = GameStage.EndOfGame;
            CameraManager.instance.TurnOffAllCameras();
            // Show end game panel
            endGameCanvas.SetActive(true);
            StartCoroutine(DeactivateEndGameCanvas(5));
            endGamePanel = endGameCanvas.transform.GetChild(0).gameObject;
            audio = endGamePanel.GetComponent<AudioSource>();
            audio.clip = gameOverSound;
            audio.Play();
        }

        currentStage = GameStage.EndOfGame;
        CameraManager.instance.TurnOffAllCameras();
        GameObject.Find("InitialSetupCanvas").SetActive(false);
        // Show end game panel
        endGameCanvas.SetActive(true);
        StartCoroutine(DeactivateEndGameCanvas(10));
        endGamePanel = endGameCanvas.transform.GetChild(0).gameObject;
        endGamePanel.GetComponentInChildren<TextMeshProUGUI>().text = winner + " wins!";
        audio = endGamePanel.GetComponent<AudioSource>();
        audio.clip = gameOverSound;
        audio.Play();

    }

    public IEnumerator DeactivateEndGameCanvas(float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        endGameCanvas.SetActive(false);
    }

    public void UpdateStage(GameStage nextStage)
    {
        actionStage = nextStage;
    }

    public void SetStateOfCameras(bool DeployCamera, bool EnemyMapCamera, bool RotateCamera, bool RotateCameraFull)
    {
        camera1.gameObject.SetActive(DeployCamera);
        camera2.gameObject.SetActive(EnemyMapCamera);
        camera3.gameObject.SetActive(RotateCamera);
        camera4.gameObject.SetActive(RotateCameraFull);
    }

}


/// <summary>
/// 07-03-2025 : Game will be
/// Deploy
/// LOOP START ( turn + 1 )
/// PlayerAttackEnemyMap
/// PlayerAttackCinematic
/// IAAttackPlayerMap
/// LOOP END
/// EndOfGame
/// </summary>
public enum GameStage
{
    Deploy = 0,
    PlayerAttackEnemyMap = 1,
    PlayerAttackCinematic = 2,
    IAAttackPlayerMap = 3,
    IAAttackCinematic = 4,
    EndOfGame = 99
}

public enum HitResult
{
    Miss = 0,
    Hit = 1,
    Sunk = 2
}
