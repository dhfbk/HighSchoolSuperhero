using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WebDebug : MonoBehaviour
{
    public static Transform windowInstance;
    public GameObject entry;
    public static GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        windowInstance = this.transform;
        prefab = entry;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void Print(string message)
    {
        GameObject msg = Instantiate(prefab);
        msg.transform.SetParent(prefab.transform.parent);
        msg.transform.localPosition = prefab.transform.localPosition;
        msg.transform.localScale = new Vector3(0.5F, 0.5F, 0.5F);
        msg.transform.localRotation = Quaternion.identity;
        msg.GetComponent<Text>().text = message;
    }
}
