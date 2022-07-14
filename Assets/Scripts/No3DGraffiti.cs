using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataUtilities;

public class No3DGraffiti : MonoBehaviour
{
    public Player agent;
    bool initialized;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (LoadUtility.AllLoaded && GetComponent<Graffiti>().graffitiLoaded && !initialized)
        {
            GetComponent<Graffiti>().Agent = agent;
            GetComponent<Graffiti>().StartGraffiti(agent);
            initialized = true;
        }
    }
}