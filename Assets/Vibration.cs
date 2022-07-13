using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vibration : MonoBehaviour
{
    Vector3 dest;
    public float range;
    Vector3 range1;
    Vector3 range2;
    // Start is called before the first frame update
    void Start()
    {
        range = range / 20f;
        range1 = new Vector3(transform.localPosition.x + range, transform.localPosition.y, transform.localPosition.z + range);
        range2 = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        dest = range1;
    }

    // Update is called once per frame
    void Update()
    {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, dest, Time.deltaTime);
            if (transform.localPosition == dest)
            {
                dest = dest == range2 ? range1 : range2;
            }
    }
}
