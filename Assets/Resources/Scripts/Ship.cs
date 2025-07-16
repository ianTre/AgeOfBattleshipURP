using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UIElements;

public class Ship : MonoBehaviour
{

    public int HealthPoints;
    public int attackPoints;
    public int defencePoints;
    public int Speed;
    public Mode attackMode = Mode.None;
    public bool hasfocus = false;
    public ShipType shipType;
    public List<Tile> ocuppiedTiles;
    public List<ShipTile> shipTiles;
    public bool isSunk = false;
    private int count = 0;
    public SelectionController selectionlight;
    public double coord = 0;
    public Tile selectedTile;
    private FirePowerController firePowerController;


    // Start is called before the first frame update
    void Start()
    {
        ocuppiedTiles = new List<Tile>();
        shipTiles = new List<ShipTile>();
        selectionlight = FindAnyObjectByType<SelectionController>();
        firePowerController = GetComponent<FirePowerController>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void DestroyShip()
    {
        PlayerController.instance.RemoveShip(this);
        Destroy(this.gameObject);
        selectionlight.SelectionLightOff(this);
    }

    public void AddOcuppiedTile(Tile tile)
    {
        if (!ocuppiedTiles.Contains(tile))
        {
            ocuppiedTiles.Add(tile);
            shipTiles.Add(new ShipTile(tile, count++));
            selectedTile = tile;
            coord = ocuppiedTiles.Average(x => x.ZCoord);
        }
    }

    public void TakeHit(Tile tile)
    {
        shipTiles.Find(x => x.tile == tile).hitted = true;
        isSunk = shipTiles.All(x => x.hitted);
    }






    public int Size()
    {
        switch (shipType)
        {
            case ShipType.Battleship:
                return 5;
            case ShipType.Corvette:
                return 2;
            case ShipType.Crusier:
                return 3;
            case ShipType.Destroyer:
                return 4;
            case ShipType.Frigate:
                return 3;
            default:
                return 1;
        }
    }

    public void RemoveOcuppiedTile(Tile tile)
    {
        ocuppiedTiles.Remove(tile);
    }


    //public void OnMouseDown()
    //{          
    //    if(hasfocus)
    //    {
    //     hasfocus = false;
    //     selectionlight.SelectionLightOff(this);
    //    }
    //    else
    //    {
    //      hasfocus = true;
    //      selectionlight.SelectionLightOn(coord, selectedTile, this); 
    //    }
    //}

}

public class ShipTile
{
    public int tileNumber;
    public Tile tile;
    public bool hitted;

    public ShipTile(Tile tile, int number)
    {
        this.tile = tile;
        this.hitted = false;
        this.tileNumber = ++number;
        Debug.Log("Tile number: " + tileNumber + "is tile: " + tile.name);
    }
}

public enum Mode
{
    None = 0,
    Attack = 1,
    Deffend = 2,
    Move = 3
}

public enum ShipType
{
    Frigate = 1,
    Destroyer = 2,
    Crusier = 3,
    Corvette = 4,
    Battleship = 5
}
