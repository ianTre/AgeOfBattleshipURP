using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSoundController : MonoBehaviour
{
    
    // Start is called before the first frame updateusing System;

    [SerializeField]
    AudioClip shipDeploySound;
    
    [SerializeField]
    AudioClip shipShootingSound; 
   
    [SerializeField]
    AudioClip waterSplashSound; 
    
    [SerializeField]
    AudioClip shipSelectSound; 


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    public void PlayShipDeploySound(Ship ship)
    {
        AudioSource audio = ship.GetComponent<AudioSource>();
        audio.clip = shipDeploySound;
        audio.Play();
    }
      public void PlayShipSelectionSoundOff(Ship ship)
    {
        AudioSource audio = ship.GetComponent<AudioSource>();
        audio.clip = shipDeploySound;
        audio.Stop();
    }

     public void PlayShipSelectionSound(Ship ship)
    {
        AudioSource audio = ship.GetComponent<AudioSource>();
        audio.clip = shipSelectSound;
        audio.Play();
    }

    public void PlayExplosionSound(Ship ship)
    {
        AudioSource audio = ship.GetComponent<AudioSource>();
        audio.clip = shipShootingSound;
        audio.Play();  
    }
}

