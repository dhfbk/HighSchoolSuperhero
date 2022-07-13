using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraffitiAssignArea : MonoBehaviour
{

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "MapArea1")
            GetComponent<Triggerer>().triggerableObject.GetComponent<Graffiti>().area = 1;
        else if (other.tag == "MapArea2")
            GetComponent<Triggerer>().triggerableObject.GetComponent<Graffiti>().area = 2;
        else if (other.tag == "MapArea3")
            GetComponent<Triggerer>().triggerableObject.GetComponent<Graffiti>().area = 3;
        else if (other.tag == "MapArea4")
            GetComponent<Triggerer>().triggerableObject.GetComponent<Graffiti>().area = 4;
        else if (other.tag == "MapArea5")
            GetComponent<Triggerer>().triggerableObject.GetComponent<Graffiti>().area = 5;

    }
}
