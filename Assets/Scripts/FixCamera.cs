using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
	}
}
