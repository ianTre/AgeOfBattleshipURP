using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class AnimationController : MonoBehaviour
{
    [SerializeField]
    GameObject missExplosion;
    [SerializeField]
    GameObject HitExplosion;
    public static AnimationController instance;
    public Vector3 currentPosition;
    public HitResult currentHitResult;
    [SerializeField]
    AudioClip BombFalling;
    [SerializeField]
    AudioClip WaterSplash;
    [SerializeField]
    AudioClip BombExplosion;
    [SerializeField]
    GameObject cameraRotatorFull;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void PlayExplosion()
    {
        currentPosition.y += 40;
        cameraRotatorFull.GetComponent<CamaraIAAtack>().StartVisualization(currentPosition);
        cameraRotatorFull.GetComponentInChildren<AudioSource>().clip = BombFalling;

        cameraRotatorFull.GetComponentInChildren<AudioSource>().Play();
        switch (currentHitResult)
        {
            case HitResult.Miss:
                PlayMissExplosion(currentPosition);
                break;
            case HitResult.Sunk:
                PlaySunkExplosion(currentPosition);
                break;
            case HitResult.Hit:
                PlayHitExplosion(currentPosition);
                break;
            default:
                break;
        }
    }

    public void SetNextExplosion(Vector3 nextPos, HitResult nextHitResult)
    {
        currentPosition = nextPos;
        currentHitResult = nextHitResult;
    }

    public void PlayMissExplosion(Vector3 position)
    {
        position.y -= 30f; //Initial value is offset + 40. For splash ( water level ) +10 looks like a good value.
        StartCoroutine(ShowMissExplosion(3f, position));
    }

    public void PlayHitExplosion(Vector3 position)
    {
        position.y -= 20f;
        StartCoroutine(ShowHitAnimation(3f, position));
    }

    public void PlaySunkExplosion(Vector3 position)
    {
        position.y -= 10f;
        StartCoroutine(ShowSunkExplosion(2f, 8f, position));
    }

    public IEnumerator ShowMissExplosion(float delay, Vector3 position)
    {
        float elapsed = 0f;
        while (elapsed < delay)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        cameraRotatorFull.GetComponentInChildren<AudioSource>().clip = WaterSplash;
        cameraRotatorFull.GetComponentInChildren<AudioSource>().Play();
        GameObject explosion = Instantiate(missExplosion, position, Quaternion.identity);
        elapsed = 0f;
        while (elapsed < 5f)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(explosion, 5f);
        GameController.instance.UpdateStage(GameStage.PlayerAttackEnemyMap);
    }


    public IEnumerator ShowHitAnimation(float delay, Vector3 position)
    {
        float elapsed = 0f;
        while (elapsed < delay)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        GameObject explosion = Instantiate(HitExplosion, position, Quaternion.identity);
        explosion.transform.localScale = new Vector3(explosion.transform.localScale.x, explosion.transform.localScale.y, explosion.transform.localScale.z);
        explosion.transform.position = new Vector3(explosion.transform.position.x, explosion.transform.position.y + 7, explosion.transform.position.z);
        explosion.GetComponent<ParticleSystem>().Play();
        cameraRotatorFull.GetComponentInChildren<AudioSource>().clip = BombExplosion;
        cameraRotatorFull.GetComponentInChildren<AudioSource>().Play();
        Destroy(explosion, 5f);

        Ship ship = PlayerController.instance.GetShipByPosition(position);
        int totalHits = ship.shipTiles.Where(x => x.hitted == true).Count();
        elapsed = 0f;
        float fireDelay = 0.5f;
        while (elapsed < fireDelay)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        ship.gameObject.GetComponent<FirePowerController>().TakeDamage(totalHits, ship.shipTiles.Count);
        elapsed = 0f;
        while (elapsed < delay)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        GameController.instance.UpdateStage(GameStage.PlayerAttackEnemyMap);
    }

    //OLD SUNK ANIMATION : You hear 1 shot , and then chain reaction explosions and final multiple explosion. OBSOLETE
    /* 
    private IEnumerator ShowSunkExplosion(float initialDelay, float betweenExplosionsDelay, float finalDelay, Vector3 position)
    {
        float elapsed = 0f;
        while (elapsed < initialDelay)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        Ship ship = PlayerController.instance.GetShipByPosition(position);
        int totalHits = ship.shipTiles.Where(x => x.hitted == true).Count();
        ship.gameObject.GetComponent<FirePowerController>().TakeDamage(totalHits, ship.shipTiles.Count);
        elapsed = 0f;
        while (betweenExplosionsDelay >= 0)
        {
            GameObject explosionsFolder = ship.gameObject.transform.Find("Explosions").gameObject;
            if (explosionsFolder == null)
            {
                Debug.Log("Explosions folder not found in ship: " + ship.name);
                yield break;
            }
            for (int i = 0; i < explosionsFolder.transform.childCount; i++)
            {
                GameObject explosion = explosionsFolder.transform.GetChild(i).gameObject;
                explosion.GetComponent<ParticleSystem>().Play();
                explosion.GetComponent<AudioSource>().Play();
                elapsed = 0f;
                while (elapsed < betweenExplosionsDelay)
                {
                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }
            betweenExplosionsDelay--;
            elapsed = 0f;
        }

        while (elapsed < finalDelay)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        cameraRotatorFull.GetComponent<CameraRotator>().StopRotation();
        GameController.instance.UpdateStage(GameStage.PlayerAttackEnemyMap);
    }*/

    private IEnumerator ShowSunkExplosion(float initialDelay ,float finalDelay, Vector3 position)
    {
        float elapsed = 0f;
        while (elapsed < initialDelay)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        GameObject explosion = Instantiate(HitExplosion, position, Quaternion.identity);
        explosion.transform.position = new Vector3(explosion.transform.position.x, explosion.transform.position.y + 7, explosion.transform.position.z);
        explosion.GetComponent<ParticleSystem>().Play();
        cameraRotatorFull.GetComponentInChildren<AudioSource>().clip = BombExplosion;
        cameraRotatorFull.GetComponentInChildren<AudioSource>().Play();
        Destroy(explosion, 3f);

        Ship ship = PlayerController.instance.GetShipByPosition(position);
        int totalHits = ship.shipTiles.Where(x => x.hitted == true).Count();
        elapsed = 0f;
        float fireDelay = 0.5f;
        while (elapsed < fireDelay)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        ship.gameObject.GetComponent<FirePowerController>().TakeDamage(totalHits, ship.shipTiles.Count);
        ship.gameObject.GetComponent<PlayableDirector>().Play();

        elapsed = 0f;
        while (elapsed < finalDelay)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        //cameraRotatorFull.GetComponent<CameraRotator>().StopRotation();
        GameController.instance.UpdateStage(GameStage.PlayerAttackEnemyMap);
    }
}
