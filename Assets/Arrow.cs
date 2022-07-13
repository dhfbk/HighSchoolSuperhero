using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Transform target;
    private Vector3 originalScale;
    private Vector3 bigScale;
    private Vector3 destScale;
    private Vector3 startScale;
    float t;
    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
        bigScale = originalScale * 1.5f;
        destScale = bigScale;
        startScale = originalScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);

        t += Time.deltaTime;
        transform.localScale = Vector3.Lerp(startScale, destScale, t);
        if (t >= 1)
        {
            if (destScale == bigScale)
            {
                destScale = originalScale;
                startScale = bigScale;
            }
            else
            {
                destScale = bigScale;
                startScale = originalScale;
            }
            t = 0;
        }
    }
}
