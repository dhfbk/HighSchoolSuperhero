using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterButtonAnimation : MonoBehaviour
{
    Vector3 upPos;
    Vector3 downPos;
    Vector3 pos;
    float t;

    private void Start()
    {
        upPos = transform.localPosition + new Vector3(0, 3f, 0);
        downPos = transform.localPosition;
        pos = downPos;
    }

    void Update()
    {
        if (t > 0.5f)
        {
            pos = pos == upPos ? downPos : upPos;
            transform.localPosition = pos;
            t = 0;
        }
        else
        {
            t += Time.deltaTime;
        }
    }
}
