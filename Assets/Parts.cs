using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Parts : MonoBehaviour
{
    public GameObject eyes;
    public GameObject glasses;
    public GameObject lenses;
    public GameObject body;
    public GameObject shirt;
    public GameObject pants;
    public GameObject shoes;
    public GameObject hair;
    public bool RandomizeAtStart;
    public bool CombineAfterRandomization;
    // Start is called before the first frame update
    void Start()
    {
        if (RandomizeAtStart)
            NPCUtilities.RandomLook(this.gameObject);
        if (CombineAfterRandomization)
            GetComponent<CombineChildren>().Combine();
    }
}
