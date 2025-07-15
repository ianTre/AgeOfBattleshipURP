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
        Debug.Log(HitIcon);
        if (collider.tag == "SweepHit")
        {
            collider.gameObject.GetComponent<RadarIconFadeoutController>().ResetAlpha();

            if (HitIcon == "HitIcon(Clone)")
            {
                collider.gameObject.GetComponent<RadarIconFadeoutController>().ResetAlpha();
                audio.Play();
            }
        }
        //else
            //    Debug.Log("wrong collider: " + collider.GetInstanceID().ToString() + " - " + collider.name + " - " + collider.tag ?? "");
        }

    private void Update()
    {

    }
}
