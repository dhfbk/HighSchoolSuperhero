using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlaceAtStart : MonoBehaviour
{
    public float y;

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
