using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDownSorting : MonoBehaviour
{
    private void OnEnable()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas)
        {
            canvas.sortingLayerName = "DropDown";

        }
    }
}
