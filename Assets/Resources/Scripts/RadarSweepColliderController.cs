using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarSweepColliderController : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "SweepHit")
            collider.gameObject.GetComponent<RadarIconFadeoutController>().ResetAlpha();
        //else
        //    Debug.Log("wrong collider: " + collider.GetInstanceID().ToString() + " - " + collider.name + " - " + collider.tag ?? "");
    }

    private void Update()
    {

    }
}
