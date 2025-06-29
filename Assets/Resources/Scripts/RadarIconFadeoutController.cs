using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RadarIconFadeoutController : MonoBehaviour
{
    public float FadeValue { get; set; } = 1;
    SpriteRenderer SpriteRenderer { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ResetAlpha()
    {
        FadeValue = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (FadeValue > 0)
            FadeValue -= 1f * Time.deltaTime;

        SpriteRenderer.color = SpriteRenderer.color.WithAlpha(FadeValue);
    }
}
