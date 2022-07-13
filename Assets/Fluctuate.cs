using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fluctuate : MonoBehaviour
{
    public Vector3 center, dest;
    RectTransform rt;
    bool called;
    public float range;
    // Start is called before the first frame update
    void Start()
    {
        range = 3f;
    }

    // Update is called once per frame
    void Update()
    {
        if (called)
        {
            rt.localPosition = Vector3.MoveTowards(rt.localPosition, dest, Time.deltaTime * 3);
            if (Vector3.Distance(dest, rt.localPosition)<0.5f)
            {
                NewPos();
            }
        }
    }

    void NewPos()
    {
        dest = new Vector3(center.x+Random.Range(-range, range), center.y+Random.Range(-range, range), rt.localPosition.z);
    }

    public void StartFluctuate()
    {
        rt = GetComponent<RectTransform>();
        center = rt.localPosition;
        dest = center;
        called = true;
    }
}
