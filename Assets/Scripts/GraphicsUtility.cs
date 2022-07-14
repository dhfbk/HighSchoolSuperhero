using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GraphicsUtility : MonoBehaviour
{
    public static void EnableDof()
    {
        PostProcessVolume vol = FindObjectOfType<PostProcessVolume>();
        vol.enabled = true;
    }

    public static void DisableDof()
    {
        PostProcessVolume vol = FindObjectOfType<PostProcessVolume>();
        vol.enabled = false;
    }
}
