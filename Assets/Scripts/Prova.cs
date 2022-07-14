using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Prova : MonoBehaviour
{
    List<Vector2> newUv;
    // Start is called before the first frame update
    void Start()
    {
        newUv = new List<Vector2>();
        //Swap
        GetComponent<SkinnedMeshRenderer>().sharedMesh.GetUVs(8, newUv);
        GetComponent<SkinnedMeshRenderer>().sharedMesh.SetUVs(0, newUv);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
