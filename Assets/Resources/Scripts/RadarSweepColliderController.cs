using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RadarSweepColliderController : MonoBehaviour
{
    [SerializeField]
    AudioClip RadarBeepSound;
    string HitIcon;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag != "SweepHit")
            return;
        HitIcon = collider.gameObject.name;
        if (HitIcon != "HitIcon(Clone)")
        {
            return;
        }
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = RadarBeepSound;
        
        //collider.gameObject.GetComponent<RadarIconFadeoutController>()?.ResetAlpha();
        //collider.gameObject.GetComponent<RadarIconFadeoutController>()?.ResetAlpha();
        audio.Play();
    }

    private void Update()
    {

    }
}
