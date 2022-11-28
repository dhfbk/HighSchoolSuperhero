using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInFadeOut : MonoBehaviour
{
    private SpriteRenderer r;

    private void Start()
    {
        r = GetComponent<SpriteRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        r.color = new Color(1, 1, 0, Mathf.Sin(Time.time*4));
    }
}
