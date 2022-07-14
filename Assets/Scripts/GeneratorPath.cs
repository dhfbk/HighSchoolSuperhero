using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorPath : MonoBehaviour
{
    private float y;
    private float starty;
    void Start()
    {
        
    }

    void Update()
    {
        transform.RotateAround(transform.parent.position, Vector3.up, Time.deltaTime*50);
        y = starty + Mathf.Sin(2*Time.time);
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
