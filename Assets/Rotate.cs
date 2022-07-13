using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    float s;
    // Start is called before the first frame update
    void Start()
    {
        s = 10;
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y+Time.deltaTime*s, transform.eulerAngles.z);
    }
}
