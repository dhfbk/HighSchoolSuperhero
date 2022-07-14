using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UnsetParentOnLoad : MonoBehaviour
{
    void Start()
    {
        transform.SetParent(null);
    }
}
