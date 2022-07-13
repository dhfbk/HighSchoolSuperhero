using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoBackToPlace : MonoBehaviour {

    public GameObject place;
    Animator animator;
    public GameObject triggerArea;
    private NPCController npcc;
    private bool free;

	// Use this for initialization
	void Start () 
    {
        animator = GetComponent<Animator>();
        npcc = GetComponent<NPCController>();
	}
	
	// Update is called once per frame
	void Update ()
    {

        if (Vector3.Distance(transform.position, place.transform.position) > 1.0F)
        {
            //animator.SetBool("Walk", true);
            //transform.LookAt(transform.parent.position);
            transform.position = Vector3.MoveTowards(transform.position, place.transform.position, Time.deltaTime);
        }
        else
        {
            //if (animator.GetBool("walk") == true)
            //    animator.SetBool("Walk", false);
        }
    }
}
