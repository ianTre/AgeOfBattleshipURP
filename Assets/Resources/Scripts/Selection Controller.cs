using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class SelectionController : MonoBehaviour 
{
    // Start is called before the first frame update
   public SelectionController selectionlight;
   public Tile tile;
   public ShipSoundController shipSounds;

    void Start()
    {
        selectionlight = FindAnyObjectByType<SelectionController>(); 
        tile = FindAnyObjectByType<Tile>();
        shipSounds = FindAnyObjectByType<ShipSoundController>(); 
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SelectionLightOff(Ship ship)
    {
        shipSounds.PlayShipSelectionSoundOff(ship);
        GameObject selectionlight = GameObject.Find("SelectionLight");
        ship.hasfocus = false;
        selectionlight.GetComponent<Light>().enabled = false;
    }

    public void SelectionLightOn(double coord, Tile selectedTile, Ship ship)
    {      
       float coordZ = (float)coord;
       coordZ = CorrectZpos(coordZ);
       shipSounds.PlayShipSelectionSound(ship);
       GameObject selectionlight = GameObject.Find("SelectionLight");
       selectionlight.GetComponent<Light>().enabled = true;
       selectionlight.transform.position = new Vector3(selectedTile.Xpos,31, coordZ);
    }

    public void SelectionLightOnSimplified(Ship ship)
    {
        Ship previousShip = PlayerController.instance.ships.Find(ship => ship.hasfocus);
        if(previousShip != null )
        {
            previousShip.hasfocus = false;
        }
        shipSounds.PlayShipSelectionSound(ship);
        ship.hasfocus = true;
        this.GetComponent<Light>().enabled = true;
        this.transform.position = new Vector3(ship.transform.position.x, 31, ship.transform.position.z);
    }


    private float CorrectZpos(float coordZ)
    {
        switch (coordZ)
        {
            case 0:
                return 90f;
            case 1:
                return 60f;
            case 2:
                return 30f;
            case 3:
                return 0f;
            case 4:
                return -30f;
            case 5:
                return -60;
            case 6:
                return -90f;
            case 7:
                return -120f;
            case 8:
                return -150f;
            case 9:
                return -180f;
            default:
                return coordZ;
        }
    }



}
