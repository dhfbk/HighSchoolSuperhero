using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DevTools : MonoBehaviour
{
    public bool DeleteSave;
    public GameObject dw;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown("x"))
        {
            dw.SetActive(!dw.activeSelf);
        }

        if (DeleteSave)
            PlayerPrefs.DeleteAll();
    }
}
