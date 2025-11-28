using System.Collections;
using System.Linq;
using UnityEngine;

public class FocusAttention : MonoBehaviour
{
    Light focusLight;
    // Start is called before the first frame update
    void Start()
    {
        focusLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SuggestTile(Tile tileToFocus)
    {

    }

    public void PreviousToShowResult(Tile tileToFocus, bool isMiss) //If isMiss is true, the attack was a miss and miss sound will be played. If false, hit sound will be played no matter if hit or sunk.
    {
        var tilePos = tileToFocus.transform.position;
        this.transform.position = new Vector3(tilePos.x, this.transform.position.y, tilePos.z);
        focusLight.enabled = true;
        StartCoroutine(ChangeLightIntensity(0, 20, 1, 0.5f, 2, isMiss));
    }

    public IEnumerator ChangeLightIntensity(float initial, float final, float growingTime, float stayingTime, float decreasingTime, bool isMiss)
    {
        Camera activeCamera = CameraManager.instance.GetActiveCamera().First();
        AudioSource audioSource = activeCamera.GetComponent<AudioSource>();
        AnimationController.instance.PlayMissileIncomingSound(audioSource);
        focusLight.range = initial;
        float elapsed = 0f;
        while (elapsed < growingTime)
        {
            focusLight.range = Mathf.Lerp(initial, final, elapsed / growingTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        elapsed = 0f;
        while (elapsed < stayingTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        elapsed = 0f;
        
        while (elapsed < decreasingTime)
        {
            focusLight.range = Mathf.Lerp(final, initial, elapsed / decreasingTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        AnimationController.instance.StopSound(audioSource);
        if (isMiss)
            AnimationController.instance.PlayMissExplotionSound(audioSource);
        else
            AnimationController.instance.PlayHitExplotionSound(audioSource);
    }


}
