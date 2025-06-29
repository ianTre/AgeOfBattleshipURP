using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public List<Ship> ships;
    public static PlayerController instance;
    public bool leftCtrlPressed = false;
    public bool cameraOnMain = true;
    public Dictionary<ShipType,string> panelForShip;
    [SerializeField]
    GameObject explosions;
    [SerializeField]
    GameObject VirtualCamera; 
    private int shipCount; 
    // private int totalShipAvailable = 5;  // Total number of ships available for deployment
    
    // Start is called before the first frame update

    void Awake ()
    {
        instance = this;
    }
    void Start()
    {
        ships = new List<Ship>();
        panelForShip = new Dictionary<ShipType, string>();
        AddConstantReferences();
    }

    private void AddConstantReferences()
    {
        panelForShip.Add(ShipType.Battleship,"Ship1Panel");
        panelForShip.Add(ShipType.Crusier,"Ship2Panel");
        panelForShip.Add(ShipType.Corvette,"Ship4Panel");
        panelForShip.Add(ShipType.Destroyer,"Ship3Panel");
        panelForShip.Add(ShipType.Frigate,"Ship5Panel");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            leftCtrlPressed=!leftCtrlPressed;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            VirtualCamera.SetActive(true);

            GameObject.Find("TimeLine").GetComponent<PlayableDirector>().Play();
            //add time 6 seconds stop the timeline         
            Debug.Log("Timeline started");
        }
    }

    /*private void SetAtackMode(bool v)
    {
        AtackRadarCanvasController.instance.CreateGrid(MapController.instance.rowSize,MapController.instance.columSize);
    }*/

    private void UpdateCameraPosition()
    {
        var CameraAnchor = GameObject.Find("CameraAnchor");
        if(CameraAnchor==null)
        {
            Debug.Log("CameraAnchor GameObject is null , check UpdateCameraPosition on PlayerController");
            return;
        }
        Vector3 newPosition;
        if(cameraOnMain)
            newPosition = MapController.instance.AllTiles.First().transform.position;
        else
        {
            var tile = EnemyMapController.instance.allTiles.FirstOrDefault();
            if(tile==null)
                return;
            newPosition = tile.transform.position;
        }

        CameraAnchor.transform.position = new Vector3(newPosition.x,newPosition.y,newPosition.z);
    }

    public void AddShip(Ship ship)
    {
        this.ships.Add(ship);
        shipCount++;
    }

    public void RemoveShip(Ship ship)
    {
        if(!ships.Contains(ship))
            return;
        
        ships.Remove(ship);
        shipCount--;
        string panelName = panelForShip[ship.shipType];
        GameObject shipPanel = GameObject.Find(panelName);
        if(shipPanel == null)
        {
            Debug.Log("Error: panel not found, check RemoveShip on PlayerController. Panel name is :" + panelName);
            return;
        }
        shipPanel.transform.Find("Inner").GetComponent<DragUIShips>().EnablePanel();

    }

    public void EndDeployStage()
    {
        if(shipCount > 0) // (totalShipAvailable == shipCount) << original condition, temporarily changed
        {
            GameController.instance.UpdateStage(GameStage.PlayerAttackEnemyMap);
        }
        else
        {
            Debug.Log("You must deploy all ships");
        }
    }



    public bool CanShipBeDeployed(Ship ship,int quantity)
    {
        return ships.Where(x => x.shipType == ship.shipType).Count() < quantity;
    }

    public HitResult ProcessEnemyHit(int z,int x)
    {
        HitResult hitResult = GetHitResult(z, x);
        Tile tile = MapController.instance.FindTileByCoord(z,x);
        if(tile == null)
        {
            Debug.Log("Error: tile not found, check ShowExplosionAnimation on PlayerController");
        }
        AnimationController.instance.SetNextExplosion(tile.transform.position,hitResult);
        return hitResult;
    }

    public Ship GetShipByPosition(Vector3 position)
    {
        foreach (Ship ship in ships)
        {
            if (ship.ocuppiedTiles.Any(tile => tile.transform.position.x == position.x && tile.transform.position.z == position.z))
            {
                return ship;
            }
        }
        return null;
    }


    private HitResult GetHitResult(int z, int x)
    {
        foreach (Ship ship in ships)
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

    public bool CheckEndOfGame()
    {
        return ships.All(s => s.isSunk);
    }

    public Ship getShipToBeActioned()
    {
        var avaibleShips = ships.Where(s => s.isSunk == false);
        int random = UnityEngine.Random.Range(0, avaibleShips.Count());
        Ship selShip = avaibleShips.ElementAt(random);
        return selShip;
    }
}
    
    
