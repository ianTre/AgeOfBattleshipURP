using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class FirePowerController : MonoBehaviour
{
    public List<GameObject> cannons;
    private Ship ship;
    public ShipSoundController shipSounds;
    public bool isFiring = false;
    public List<GameObject> fireSpots;
    void Start()
    {
        ship = GetComponent<Ship>();
        shipSounds = FindAnyObjectByType<ShipSoundController>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FireCannons()
    {
        StartCoroutine(FireSubRoutine());
    }

    private IEnumerator FireSubRoutine()
    {
        isFiring = true;
        foreach (var cannon in cannons)
        {
            var explosion = cannon.GetComponentInChildren<ParticleSystem>();
            if (explosion == null)
            {
                Debug.Log("Explosion not found in cannon: " + cannon.name);
                continue;
            }
            //Vector3 cannonPosition = new Vector3(cannon.transform.position.x, cannon.transform.position.y, cannon.transform.position.z + 15);
            //var explosion = Instantiate(explosionPrefab, cannonPosition , cannon.transform.rotation);
            explosion.Play();
            shipSounds.PlayExplosionSound(ship);
            MoveCannonBackAndforward(cannon.transform);
            yield return new WaitForSeconds(1.5f); // Adjust the delay as needed

        }
        isFiring = false;
    }

    private void MoveCannonBackAndforward(Transform cannon)
    {
       Vector3 startPosition = new Vector3(cannon.transform.localPosition.x,cannon.transform.localPosition.y,cannon.transform.localPosition.z);
       Vector3 finalPosition = new Vector3(cannon.transform.localPosition.x,cannon.transform.localPosition.y,cannon.transform.localPosition.z - 2);
       StartCoroutine(Lerper(startPosition, finalPosition, 0.3f, cannon));
    }

    IEnumerator Lerper(Vector3 start , Vector3 end, float duration, Transform cannon)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            cannon.transform.localPosition = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cannon.transform.localPosition = end;
        elapsed = 0f;
        while (elapsed < duration)
        {
            cannon.transform.localPosition = Vector3.Lerp(end,start, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cannon.transform.localPosition = start;
    }
    public void TakeDamage(int totalHits, int totalSizeOfShip)
    {
        if(totalHits == totalSizeOfShip)
        {
            var scale =fireSpots[0].transform.localScale;
            var newScale = new Vector3(scale.x * 2, scale.y * 2, scale.z * 2);
            foreach (var spot in fireSpots)
            {
                spot.transform.localScale = newScale;
                spot.GetComponent<ParticleSystem>().Play();
            }
            Debug.Log("Ship is sunk");
        }
        
        if (totalSizeOfShip - totalHits == 1)
        {
            //Ship is one hit away from sinking
            //Play all fire animations and double size of fire
            var scale =fireSpots[0].transform.localScale;
            var newScale = new Vector3(scale.x * 2, scale.y * 2, scale.z * 2);
            foreach (var spot in fireSpots)
            {
                spot.transform.localScale = newScale;
                spot.GetComponent<ParticleSystem>().Play();
            }
        }

        if (totalSizeOfShip - totalHits == 2)
        {
            //Ship is two hits away from sinking
            //Play all fire animations
            foreach (var spot in fireSpots)
            {
                spot.GetComponent<ParticleSystem>().Play();
            }
        }

        if (totalSizeOfShip - totalHits == 3)
        {
            //Ship is Three hits away from sinking
            //Play half of fire animations
            int halfFire = fireSpots.Count / 2;
            for (int i = 0; i < halfFire; i++)
            {
                fireSpots[i].GetComponent<ParticleSystem>().Play();
            }
        }

        if (totalSizeOfShip - totalHits == 4)
        {
            //Ship is Three hits away from sinking
            //Play 25% of fire animations
            int halfFire = fireSpots.Count / 4;
            for (int i = 0; i < halfFire; i++)
            {
                fireSpots[i].GetComponent<ParticleSystem>().Play();
            }
        }
        

    }
}
