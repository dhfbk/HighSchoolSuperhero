﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debugdraw : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, new Vector3(1, 0, 0), Color.red);
        Debug.DrawLine(transform.position, new Vector3(0, 1, 0), Color.green);
        Debug.DrawLine(transform.position, new Vector3(0, 0, 1), Color.blue);
    }
}
