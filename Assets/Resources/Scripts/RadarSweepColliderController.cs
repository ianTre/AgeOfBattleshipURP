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
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = RadarBeepSound;
        HitIcon = collider.gameObject.name;
        if (collider.tag == "SweepHit")
        {
            collider.gameObject.GetComponent<RadarIconFadeoutController>()?.ResetAlpha();

            if (HitIcon == "HitIcon(Clone)")
            {
                collider.gameObject.GetComponent<RadarIconFadeoutController>()?.ResetAlpha();
                audio.Play();
            }
        }
    }

    private void Update()
    {

    }
}
