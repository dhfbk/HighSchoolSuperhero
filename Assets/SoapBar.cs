using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoapBar : MonoBehaviour
{
    public float max;
    private float soap;

    public float Soap { get => soap; 
        set
        {
            if (value < 0)
                soap = 0;
            else if (value > max)
                soap = max;
            else
                soap = value;
            UpdateBar();
        } 
    }


    public void UpdateBar()
    {
        transform.localScale = new Vector3(transform.localScale.x, Soap / max, transform.localScale.z);
    }
    public void NormalSize()
    {
        transform.parent.localScale = new Vector3(2,2,0);
    }

    public void SetNewMax(float max)
    {
        transform.parent.localScale = new Vector2(2, 2*(max/100f));
        this.max = max;
        Soap = max;
    }
}
