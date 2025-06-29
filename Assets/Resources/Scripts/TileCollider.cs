using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCollider : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ship")
        {
            Tile tile = GetComponent<Tile>();
            other.gameObject.GetComponent<Ship>().AddOcuppiedTile(tile);
        }
    }
}
