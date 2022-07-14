using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetToggles : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset()
    {
        foreach(Transform t in transform.parent.transform)
        {
            if (t.GetComponent<Toggle>())
                t.GetComponent<Toggle>().isOn = false;

        }
    }
}
