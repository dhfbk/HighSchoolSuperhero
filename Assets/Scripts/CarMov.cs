using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class CarMov : MonoBehaviour
{ 
    [Header("Destinations")]
    public GameObject[] dests;
    GameObject dest;
    
    [Header("Car specs")]
    public float speed = 5f;
    public float rotationSpeed = 0.5f;

    public GameObject[] wheelLeft;
    
    
   
    private int destIndex = 0;
    public GameObject[] wheels;
    private float wheelAngle;
    
    // Start is called before the first frame update
    void Start()
    {
        dest = dests[destIndex];
        transform.LookAt(dest.transform.position);
        
    }

    //FixedUpdate can run once, zero, or several times per frame, depending on how many physics frames per second are set in the time settings, and how fast/slow the framerate is.
    //It's for this reason that FixedUpdate should be used when applying forces, torques, or other physics-related functions - because you know it will be executed exactly in sync with the physics engine itself
    void FixedUpdate()
    {
        //transform.position += transform.forward*Time.deltaTime*5;
        transform.position = Vector3.MoveTowards(transform.position, dest.transform.position, Time.deltaTime * speed);
        if (Vector3.Distance(dest.transform.position, transform.position) < 0.50f)
        {
            if (destIndex < dests.Length - 1)
            {
                destIndex++;
            }
            else
            {
                destIndex = 0;
            }

            dest = dests[destIndex];
            //transform.LookAt(dest.transform.position);
            {
                
            }
            
            //transform.localRotation = Quaternion.Lerp(transform.localRotation,new Quaternion(0, 0.25f, 0, 0.95f), Time.deltaTime);
        }
        
        Vector3 relativePos = dest.transform.position - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        wheelAngle += speed;

        //transform.localRotation = Quaternion.Lerp(transform.localRotation,
            //new Quaternion(0, 0.25f, 0, 0.95f), Time.deltaTime);
    }
}
