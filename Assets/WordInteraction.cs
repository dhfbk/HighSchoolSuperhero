using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordInteraction : MonoBehaviour {
    RaycastHit hit;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Destroy(hit.transform.gameObject);
        }
    }
}
