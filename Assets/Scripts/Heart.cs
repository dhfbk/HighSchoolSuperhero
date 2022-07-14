using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    Vector3 scale;
    Vector3 endScale;
    Vector3 startRot;
    Coroutine currentRoutine;
    // Start is called before the first frame update
    void Start()
    {
        scale = transform.localScale;
        endScale = new Vector3(transform.localScale.x, transform.localScale.y * 2, transform.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartAnimation()
    {
        if (currentRoutine == null)
            currentRoutine = StartCoroutine(Animate());
    }    

    public IEnumerator Animate()
    {
        transform.localScale = scale;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime*3;
            transform.localScale = Vector3.Lerp(scale, endScale, t);
            if (t < 0.5f)
            {
                transform.localRotation = Quaternion.Lerp(Quaternion.Euler(startRot), Quaternion.Euler(startRot + new Vector3(0,0,25)), t*2);
            }   
            else
            {
                transform.localRotation = Quaternion.Lerp(Quaternion.Euler(startRot), Quaternion.Euler(startRot + new Vector3(0, 0, 25)), (t-0.5f) * 2);
            }
            yield return null;
        }

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime*3;
            transform.localScale = Vector3.Lerp(endScale, scale, t);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(startRot), t);
            yield return null;
        }
        currentRoutine = null;
    }
}
