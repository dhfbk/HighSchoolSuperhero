using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObstacles : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    public LayerMask layerMask;
    GameObject memObj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        //if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        //{
        //    if (memObj == null || memObj != hit.transform.gameObject)
        //    {
        //        GameObject[] meshes;

        //        if (memObj)
        //        {
        //            meshes = memObj.GetComponent<MaterialsToChange>().meshes;
        //            for (int i = 0; i < meshes.Length; i++)
        //            {
        //                meshes[i].GetComponent<Renderer>().material.shader = Shader.Find("MK/Toon/Free");
        //            }
        //        }

        //            meshes = hit.transform.gameObject.GetComponent<MaterialsToChange>().meshes;
        //            memObj = hit.transform.gameObject;
        //            for (int i = 0; i < meshes.Length; i++)
        //            {
        //                meshes[i].GetComponent<Renderer>().material.shader = Shader.Find("MK/Toon/FreeTransp");
        //            }

        //    }
        //}
        //else
        //{
        //    if (memObj != null)
        //    {
        //        GameObject[] meshes;
        //        meshes = memObj.GetComponent<MaterialsToChange>().meshes;
        //        for (int i = 0; i < meshes.Length; i++)
        //        {
        //            meshes[i].GetComponent<Renderer>().material.shader = Shader.Find("MK/Toon/Free");
        //        }
        //        memObj = null;
        //    }
        //}
    }
}
