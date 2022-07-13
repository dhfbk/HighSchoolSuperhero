using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour
{
    protected Animator Animator;

    protected Rigidbody Rigidbody;

    protected BoxCollider BoxCollider;

    protected Dog dog;
    
    // Start is called before the first frame update
    void Awake()
    {
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
        BoxCollider = GetComponent<BoxCollider>();
        dog = GetComponent<Dog>();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(Vector3.forward/(20/transform.localScale.x));
    }
    
}
