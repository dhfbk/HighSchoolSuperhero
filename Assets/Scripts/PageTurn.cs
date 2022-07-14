using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataUtilities;
[ExecuteInEditMode]
public class PageTurn : MonoBehaviour
{
    Material mat;
    float coef;
    public GameObject world;
    float t;
    float a;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
        a = mat.GetFloat("_A");
    }

    // Update is called once per frame
    void Update()
    {
        //if (LoadUtility.AllLoaded)
        //{
        //    //coef += Time.deltaTime / 30;
        //    transform.Rotate(0, 0, Time.deltaTime * 30, Space.Self);
        //    coef = Vector3.Dot(transform.right, Vector3.up)/10;

        //        mat.SetFloat("_Coefficient", coef);

        //}
        float dot = Vector3.Dot(transform.right, Vector3.up);
        transform.Rotate(0, 0, Time.deltaTime);

        t += Time.deltaTime;
        mat.SetFloat("_Theta", -1.5f+dot);
    }
}
