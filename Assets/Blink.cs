using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    public float t;
    GameObject eyes;
    bool closed;
    // Start is called before the first frame update
    void Start()
    {
        eyes = GameObject.FindGameObjectWithTag("ModelEyeL");
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (!closed)
        {
            if (t > 7)
            {              
                eyes.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("EyesClosed");
                closed = true;
                t = 0;
            }
        }
        else
        {
            if (t>0.1)
            {
                eyes.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Eyes0");
                closed = false;
                t = 0;
            }
        }
    }
}
