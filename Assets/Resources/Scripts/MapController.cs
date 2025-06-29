using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MapController : MonoBehaviour
{
    GameObject map;
    public List<Tile> AllTiles = new List<Tile>();
    public int mapSize = 0;
    public int rowSize;
    public int columSize;
    public static MapController instance;
    private bool IsDragAndDroping = false;
    private List<Tile> tilesToBeOccuppied ;
    
    //IMPORTANT: WE WILL ALWAYS FOLLOW THE PATTERN (ROW , COLUMN) , so it will be (Z , X) .

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        tilesToBeOccuppied = new List<Tile>();
        map = this.gameObject;
        MapAllTiles();
        this.rowSize = AllTiles.OrderBy(tile => tile.ZCoord).Last().ZCoord;
        this.columSize = AllTiles.OrderBy(tile => tile.XCoord).Last().XCoord;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
        private void MapAllTiles()
    {
        foreach (Transform child in map.transform)
        {
            Tile tile =child.GetComponent<Tile>();

            tile.Xpos = tile.transform.position.x;
            tile.Ypos = tile.transform.position.y;
            tile.Zpos = tile.transform.position.z;
            tile.Name = tile.gameObject.name;
            AllTiles.Add(tile);
        }
        var orderedTiles = AllTiles.OrderByDescending(tile => tile.Zpos).ThenBy(tile => tile.Xpos);
        
        int x = -1;
        int z = 0;
        float lastRowPos = orderedTiles.First().Zpos;
        foreach (Tile tile in orderedTiles)
        {
            mapSize++;
            if(tile.Zpos == lastRowPos) //Still on the same Row
                x++;    
            else // switched to a new row
            {
                x = 0;
                z++;
            }
            tile.XCoord = x;
            tile.ZCoord = z;
            lastRowPos = tile.Zpos;
            tile.Id = mapSize;
        }
    }


    public void TileIsBeingFocused(Tile tile)
    {
        if(!IsDragAndDroping)
            tile.HighlightMainColor();
    }

    public bool CanShipBeDeployed(Tile originalTile,Ship newShip)
    {
        CleanTiles();

        if (IsDragAndDroping)
        {
            if (newShip == null)
                return false;
        }

        int sizeToBeOcuppied = newShip.Size();

        int offset = 1; //1/2 == 1 . So we will increment  by [+1,-1,+2,-2,+3,-3]
        tilesToBeOccuppied.Add(FindTileByCoord(originalTile.ZCoord, originalTile.XCoord));
        sizeToBeOcuppied--;
        bool plus = true;
        offset++;
        bool verticallyOriented = PlayerController.instance.leftCtrlPressed;
        while (sizeToBeOcuppied > 0)
        {
            int auxXCoord = originalTile.XCoord;
            int auxZCoord = originalTile.ZCoord;
            if (verticallyOriented)
            {
                if (plus)
                {
                    auxXCoord = originalTile.XCoord + (offset / 2);
                }
                else
                {
                    auxXCoord = originalTile.XCoord - (offset / 2);
                }
            }
            else
            {
                if (plus)
                {
                    auxZCoord = originalTile.ZCoord + (offset / 2);
                }
                else
                {
                    auxZCoord = originalTile.ZCoord - (offset / 2);
                }
            }
            offset++;
            plus = !plus;
            sizeToBeOcuppied--;
            AddIfExists(tilesToBeOccuppied, auxZCoord, auxXCoord);
        }
        //Add if exists will not add any tile outside the map. If the numbers dont match , you are trying to add a ship outside longer that the limits of the map
        if (tilesToBeOccuppied.Count != newShip.Size())
            return false;


        foreach (Ship ship in PlayerController.instance.ships)
        {
            if (ship.ocuppiedTiles.Exists(tile => tilesToBeOccuppied.Contains(tile)))
                return false;
        }

        foreach (Tile tile in tilesToBeOccuppied)
        {
            tile.HighlightSecondaryColor();
        }
        return true;
    }

    public void CleanTiles()
    {
        if (tilesToBeOccuppied.Count > 0)
        {
            foreach (Tile tile in tilesToBeOccuppied)
            {
                tile.DeHighlighMe();
            }
            tilesToBeOccuppied.Clear();
        }
    }

    public void SetDragAndDroping(bool boolean)
    {
        this.IsDragAndDroping = boolean;
    }

    public List<Tile> getNeighborhoods(Tile tile)
    {
        int lowerX,higherX;
        int lowerZ,higherZ;
        lowerX= tile.XCoord-1;
        higherX= tile.XCoord+1;
        lowerZ= tile.ZCoord-1;
        higherZ= tile.ZCoord+1;   

        //Secuence will be 1 |  2   | 3
        //                 4 | tile | 6
        //                 7 |  8   | 9
        
        List<Tile> neighborhoods = new List<Tile>();
        AddIfExists(neighborhoods,lowerZ,lowerX);       // 1
        AddIfExists(neighborhoods,lowerZ,tile.XCoord);  // 2
        AddIfExists(neighborhoods,lowerZ,higherX);      // 3 

        AddIfExists(neighborhoods,tile.ZCoord,lowerX);  // 4
                                                        // Tile
        AddIfExists(neighborhoods,tile.ZCoord,higherX); // 6

        AddIfExists(neighborhoods,higherZ,lowerX);      // 7
        AddIfExists(neighborhoods,higherZ,tile.XCoord); // 8
        AddIfExists(neighborhoods,higherZ,higherX);     // 9

        return neighborhoods;

    }


    /// <summary>
    /// Search for a tile by coordinates and if its found it addit to the list. Be aware that Z comes first (row) and X comes second (Column)
    /// </summary>
    /// <param name="z"></param>
    /// <param name="x"></param>
    /// <returns></returns>
    private void AddIfExists(List<Tile> list, int z, int x)
    {
        Tile tile = FindTileByCoord(z,x);
        if(tile != null)
            list.Add(tile);
    }

    /// <summary>
    /// Search for a tile by coordinates. Be aware that Z comes first (row) and X comes second (Column). 
    /// </summary>
    /// <param name="z"></param>
    /// <param name="x"></param>
    /// <returns>Returns the tile or null if its not found</returns>
    public Tile FindTileByCoord(int z,int x)
    {
        return AllTiles.FirstOrDefault(tile => tile.XCoord == x && tile.ZCoord == z);
    }
}
