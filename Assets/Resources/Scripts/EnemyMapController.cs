using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Resources.Scripts;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Input = UnityEngine.Input;
using Random = UnityEngine.Random;

public class EnemyMapController : MonoBehaviour
{
    private int offsetInMap = 3000;
    public static EnemyMapController instance;
    public List<Tile> allTiles;
    GameObject enemyMap;
    GameObject EnemyShipsGO;
    public List<Ship> enemyShips;
    [SerializeField]
    GameObject radar;
    [SerializeField]
    Vector3 radarPosOffset;
    public List<Tile> enemyMapShootedTiles;
    public Tile selectedTile;
    [SerializeField]
    GameObject missSprite;
    [SerializeField]
    GameObject hitSprite;
    [SerializeField]
    GameObject sunkSprite;
    public List<Tile> PlayerMapShootedTiles;

    public List<Tile> PlayerMapShotTiles;
    public bool shipshotachieved = false;
    [SerializeField]
    List<Coord> possiblecoordHits;
    [SerializeField]
    List<Coord> successfulCoordHits;
    [SerializeField]
    AvatarController avatarController;
    private Ship sunkShip;
    private int rowNumber;
    private int columnNumber;
    public bool firingButtonEnabled = true;
    public Ship enemyShipHitted;
    public bool firingWaitEnded = false;

    [SerializeField]
    public Light focusLight;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update 
    void Start()
    {
        EnemyShipsGO = GameObject.Find("EnemyShips");
        enemyMap = GameObject.Find("EnemyMap");
        if (enemyMap == null || EnemyShipsGO == null)
            Debug.Log("Error at finding GameObjects in EnemyMapController. Check Start code");
        var enemyPos = enemyMap.transform.position;
        enemyMap.transform.position = new Vector3(enemyPos.x, enemyPos.y, enemyPos.z + offsetInMap);
        enemyMapShootedTiles = new List<Tile>();
        PlayerMapShotTiles = new List<Tile>();
        possiblecoordHits = new List<Coord>();
        successfulCoordHits = new List<Coord>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ShipStoppedFiring()
    {
        firingWaitEnded = true;
    }
    public void PlayerFiringAction()
    {
        if (!firingButtonEnabled)
            return;
        if (selectedTile == null || enemyMapShootedTiles.Contains(selectedTile))
        {
            Debug.Log("no Selected tile");
            return;
        }
        enemyMapShootedTiles.Add(selectedTile);
        CameraManager.instance.ChangeToPlayerAtacckIARadarStage2();
        GameObject prefab = CheckSpotInMap(selectedTile);
        selectedTile.DeHighlighMe();
        StartCoroutine(ShowPlayerShotResultInEnemyMap(prefab));
        GameController.instance.UpdateStage(GameStage.PlayerAttackCinematic);
    }

    private void ShowAvatarMessage(GameObject prefab)
    {
        if (prefab == missSprite)
        {
            GameController.instance.UpdateStage(GameStage.PlayerAttackCinematicFinished);
            return;
        }
        if (prefab == hitSprite)
        {
            if (enemyShipHitted.shipTiles.Count(tile => tile.hitted) == 1)
            {
                avatarController.DisplayPlayerHitAShip();
            }
            else
            {
                GameController.instance.UpdateStage(GameStage.PlayerAttackCinematicFinished);
            }
        }
        else if (prefab == sunkSprite)
        {
            if(CheckEndOfGame())
            {
                avatarController.DisplayPlayerBattleWon();
            }
            else
            { 
                avatarController.DisplayPlayerSunkAShip();
            }
        }
    }

    IEnumerator ShowPlayerShotResultInEnemyMap(GameObject prefab)
    {
        while (!firingWaitEnded)
        {
            yield return null;
        }
        firingWaitEnded = false;
        focusLight.GetComponent<FocusAttention>().PreviousToShowResult(selectedTile, prefab == missSprite);
        yield return new WaitForSeconds(3.5f);
        Instantiate(prefab, selectedTile.transform);
        if (prefab == sunkSprite)
        {
            UpdateAllOtherSprites(selectedTile, prefab);
        }
        ShowAvatarMessage(prefab);
    }

    private void UpdateAllOtherSprites(Tile selectedTile, GameObject sunkPrefab)
    {
        //Step 1. Find enemy ship using selectedTile and save it in variable sunkShip.  -> tip : check logic in CheckSpotInMap method for help.
        foreach (Ship ship in enemyShips)
        {
            if (ship.isSunk && sunkPrefab != null)
            {
                if (ship.ocuppiedTiles.Any(tile => tile.ZCoord == selectedTile.ZCoord && tile.XCoord == selectedTile.XCoord))
                {
                    sunkShip = ship;
                    break;
                }
            }
        }
        //Step 2  var allHittedTiles = FindAllTilesOfShipThatAreHitted(sunkShip);   -> Create a new method that finds all tiles from that ship. you can get the ship tiles from the ship.shipTiles list.
        List<Tile> allHittedTiles = FindAllTilesOfShipThatAreHitted(sunkShip);
        //Step 3. Loop through allHittedTiles and for each tile instantiate the sunkSprite prefab that is stored in sunkPrefab. Check line 103 to see how to instantiate the prefab in the tile.
        //step 4  Destroy or hide the previous hit sprites in those tiles to avoid overlapping.
        foreach (Tile tile in allHittedTiles)
        {
            Instantiate(sunkPrefab, tile.transform);
            var hitIcon = tile.transform.Find("HitIcon(Clone)");
            if (hitIcon != null)
                Destroy(hitIcon.gameObject);
        }
    }

    private List<Tile> FindAllTilesOfShipThatAreHitted(Ship sunkShip)
    {
        List<Tile> hittedTile = sunkShip.ocuppiedTiles;
        return hittedTile;
    }

    public GameObject CheckSpotInMap(Tile tile)
    {
        foreach (Ship ship in enemyShips)
        {
            Tile hittedTile = ship.ocuppiedTiles.Find(Stile => Stile.ZCoord == tile.ZCoord && Stile.XCoord == tile.XCoord);
            if (hittedTile != null)
            {
                ship.TakeHit(hittedTile);
                enemyShipHitted = ship;
                return ship.isSunk ? sunkSprite : hitSprite;
            }
        }
        return missSprite;
    }


    public HitResult ProcessEnemyHit(int z, int x)
    {
        foreach (Ship ship in enemyShips)
        {
            Tile hitTile = ship.ocuppiedTiles.Find(tile => tile.ZCoord == z && tile.XCoord == x);
            if (hitTile != null)
            {
                ship.TakeHit(hitTile);
                if (ship.isSunk)
                    return HitResult.Sunk;
                return HitResult.Hit;
            }
        }
        return HitResult.Miss;
    }

    public void GenerateEnemyMap(List<Tile> originalTiles)
    {
        foreach (Tile tile in originalTiles)
        {
            Vector3 newpos = new Vector3(tile.Xpos, tile.Ypos, tile.Zpos + offsetInMap);
            Instantiate(tile, newpos, Quaternion.identity, enemyMap.transform);
        }
        MapAllTiles();
        var enemyPos = enemyMap.transform.position;
        radar.transform.position = new Vector3(enemyPos.x + radarPosOffset.x, 20f + radarPosOffset.y, enemyPos.z + radarPosOffset.z);
    }

    public void MapAllTiles()
    {
        foreach (Transform child in enemyMap.transform)
        {
            Tile tile = child.GetComponent<Tile>();
            tile.isEnemyTile = true;
            tile.Xpos = tile.transform.position.x;
            tile.Ypos = tile.transform.position.y;
            tile.Zpos = tile.transform.position.z;
            tile.Name = tile.gameObject.name;
            allTiles.Add(tile);
        }

        var orderedTiles = allTiles.OrderByDescending(tile => tile.Zpos).ThenBy(tile => tile.Xpos);

        int x = -1;
        int z = 0;
        float lastRowPos = orderedTiles.First().Zpos;
        foreach (Tile tile in orderedTiles)
        {
            if (tile.Zpos == lastRowPos) //Still on the same Row
                x++;
            else // switched to a new row
            {
                x = 0;
                z++;
            }
            tile.XCoord = x;
            tile.ZCoord = z;
            /*var textMesh = tile.transform.GetComponentInChildren<TextMesh>();
            textMesh.text = "(" + z + "," + x + ")"; // (row,column)*/
            lastRowPos = tile.Zpos;
        }
    }

    public void GenerateEnemyShips(List<Ship> playerShipsToAddAsEnemy)
    {
        StartCoroutine(CoRuGenerateEnemyShips(playerShipsToAddAsEnemy));
    }

    public IEnumerator CoRuGenerateEnemyShips(List<Ship> playerShipsToAddAsEnemy)
    {
        foreach (Ship ship in playerShipsToAddAsEnemy)
        {
            bool foundRightSpot = false;
            Ship newShip = null;
            while (!foundRightSpot)
            {
                try
                {
                    bool VerticalOrientation = Random.Range(0, 2) == 0;
                    int rowNumber = Random.Range(0, MapController.instance.rowSize);
                    int columnNumber = Random.Range(0, MapController.instance.columSize);
                    Tile tile = FindTileByCoord(rowNumber, columnNumber);
                    if (tile == null)
                    {
                        Debug.Log("Check GenerateEnemyShips on EnemyMapController , wrong tile coords were generated");
                        continue;
                    }


                    #region Create a new instance of the ship
                    Quaternion quaternion;
                    if (!VerticalOrientation)
                        quaternion = Quaternion.Euler(new Vector3(0, 90, 0)); //ROTATED TO HORIZONTAL
                    else
                        quaternion = Quaternion.identity; // ROTATED VERTICALLY

                    Vector3 position = new Vector3(tile.Xpos, tile.Ypos + 6, tile.Zpos);
                    if (ship.Size() % 2 == 0) //If the ship is even, we need to adjust the position to the center of the tile
                    {
                        if (!VerticalOrientation)
                            position.x += tile.transform.localScale.x / 2;
                        else
                            position.z -= tile.transform.localScale.z / 2;
                    }
                    Vector3 newPosition = position;

                    newShip = Instantiate(ship, newPosition, quaternion, EnemyShipsGO.transform);
                    newShip.ocuppiedTiles.Clear();

                    if (!CanShipBeDeployed(tile, newShip, !VerticalOrientation))
                    {
                        foundRightSpot = false;
                        Destroy(newShip.gameObject);
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
                while (newShip.ocuppiedTiles.Count == 0)
                {
                    yield return null;
                }
                foundRightSpot = CheckNewPosition(newShip);
                #endregion
            }
            enemyShips.Add(newShip);
            SetEnemyShipLayerRecursive(newShip.gameObject);
        }


    }

    private bool CheckNewPosition(Ship newShip)
    {
        List<Tile> newTiles = newShip.ocuppiedTiles;
        List<Tile> enemyTiles = enemyShips.SelectMany(ship => ship.ocuppiedTiles).ToList();

        if (newTiles.Any(tile => enemyTiles.Exists(et => et.ZCoord == tile.ZCoord && et.XCoord == tile.XCoord))) // Is overlaping with another ship
        {
            Destroy(newShip.gameObject);
            return false;
        }
        if (newShip.ocuppiedTiles.Count != newShip.Size()) //Is doing overflow outside the map
        {
            Destroy(newShip.gameObject);
            return false;
        }
        return true;

    }

    public bool CanShipBeDeployed(Tile originalTile, Ship newShip, bool verticallyOriented)
    {
        List<Tile> tilesToBeOccuppied = new List<Tile>();
        int sizeToBeOcuppied = newShip.Size();

        int offset = 1; //1/2 == 1 . So we will increment  by [+1,-1,+2,-2,+3,-3]
        tilesToBeOccuppied.Add(FindTileByCoord(originalTile.ZCoord, originalTile.XCoord));
        sizeToBeOcuppied--;
        bool plus = true;
        offset++;
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
        {
            return false;
        }


        foreach (Ship ship in enemyShips)
        {
            if (ship.ocuppiedTiles.Exists(tile => tilesToBeOccuppied.Contains(tile)))
            {
                return false;
            }
        }
        return true;
    }

    private void SetEnemyShipLayerRecursive(GameObject _go)
    {
        _go.layer = LayerMask.NameToLayer("EnemyShips");
        foreach (Transform child in _go.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("EnemyShips");

            Transform _HasChildren = child.GetComponentInChildren<Transform>();
            if (_HasChildren != null)
                SetEnemyShipLayerRecursive(child.gameObject);
        }
    }

    private void AddIfExists(List<Tile> list, int z, int x)
    {
        Tile tile = FindTileByCoord(z, x);
        if (tile != null)
        {
            list.Add(tile);
        }
    }

    /// <summary>
    /// Search for a tile by coordinates. Be aware that Z comes first (row) and X comes second (Column). 
    /// </summary>
    /// <param name="z"></param>
    /// <param name="x"></param>
    /// <returns>Returns the tile or null if its not found</returns>
    public Tile FindTileByCoord(int z, int x)
    {
        return allTiles.FirstOrDefault(tile => tile.XCoord == x && tile.ZCoord == z);
    }

    public void TileFocus(Tile tile)
    {
        if (enemyMapShootedTiles.Contains(tile))
        {
            return;
        }
        selectedTile?.DeHighlighMe();
        selectedTile = tile;
        selectedTile.HighlightMainColor();
    }

    public void ClearTileSelection()
    {
        if (selectedTile == null)
            return;
        selectedTile?.DeHighlighMe();
        selectedTile = null;
    }
    public void IAEnemyShot()
    {
        Coord coord = null;
        if (shipshotachieved)
        {
            coord = possiblecoordHits[Random.Range(0, possiblecoordHits.Count)];
            rowNumber = coord.row;
            columnNumber = coord.column;
            possiblecoordHits.Remove(coord);
        }
        else
        {
            bool virginSpot = false;

            while (!virginSpot)
            {
                rowNumber = Random.Range(0, MapController.instance.rowSize);
                columnNumber = Random.Range(0, MapController.instance.columSize);
                var tile = PlayerMapShotTiles.Find(tile => tile.ZCoord == rowNumber && tile.XCoord == columnNumber);
                if (tile == null)
                {
                    virginSpot = true;
                }
            }
        }

        HitResult hitresult = PlayerController.instance.ProcessEnemyHit(rowNumber, columnNumber);
        Tile newTile = MapController.instance.FindTileByCoord(rowNumber, columnNumber);
        PlayerMapShotTiles.Add(newTile);



        if (hitresult == HitResult.Hit)
        {
            if (!shipshotachieved) // primera vez que se acierta
            {
                AddPossibleTarget(rowNumber + 1, columnNumber);
                AddPossibleTarget(rowNumber, columnNumber + 1);
                AddPossibleTarget(rowNumber - 1, columnNumber);
                AddPossibleTarget(rowNumber, columnNumber - 1);
                shipshotachieved = true;
                coord = new Coord(rowNumber, columnNumber);
                successfulCoordHits.Add(coord);
            }
            else
            {
                Coord originalHit = successfulCoordHits.First();
                int rowpossibleHit = originalHit.row - rowNumber;
                int columnpossibleHit = originalHit.column - columnNumber;
                successfulCoordHits.Add(coord);

                possiblecoordHits.Clear();

                if (rowpossibleHit == 0) // fila acertada por segunda vez
                {
                    int columnMaxNum = successfulCoordHits.Max(x => x.column) + 1;
                    int columnMinNum = successfulCoordHits.Min(x => x.column) - 1;
                    AddPossibleTarget(rowNumber, columnMaxNum);
                    AddPossibleTarget(rowNumber, columnMinNum);
                }
                else // columna acertada por segunda vez
                {
                    int rowMaxNum = successfulCoordHits.Max(x => x.row) + 1;
                    int rowMinNum = successfulCoordHits.Min(x => x.row) - 1;
                    AddPossibleTarget(rowMaxNum, columnNumber);
                    AddPossibleTarget(rowMinNum, columnNumber);
                }

            }

        }
        if (hitresult == HitResult.Sunk || (shipshotachieved && possiblecoordHits.Count == 0))
        {
            shipshotachieved = false;
            possiblecoordHits.Clear();
            successfulCoordHits.Clear();
        }
    }

    private void AddPossibleTarget(int rowNumber, int columnNumber)
    {
        var tile = PlayerMapShotTiles.Find(tile => tile.ZCoord == rowNumber && tile.XCoord == columnNumber);

        if (MapController.instance.FindTileByCoord(rowNumber, columnNumber) == null || tile != null)
        {
            return;
        }
        possiblecoordHits.Add(new Coord(rowNumber, columnNumber));

    }

    internal bool CheckEndOfGame()
    {
        return enemyShips.All(s => s.isSunk);
    }
}
public class Coord : MonoBehaviour
{
    public int row;
    public int column;
    public Coord(int row, int column)
    {
        this.row = row;
        this.column = column;
    }
}