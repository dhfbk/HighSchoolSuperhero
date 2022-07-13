using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : MonoBehaviour, ITriggerable
{
    public Player Agent { get; set; }
    bool over;
    bool open;
    Quaternion closeRot;
    Quaternion openRot;
    MouseOver mO;
    public float t;
    // Start is called before the first frame update
    void Start()
    {
        closeRot = transform.localRotation;
        openRot = Quaternion.Euler(new Vector3(270,0,0));
        mO = GetComponent<MouseOver>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (mO.on)
            {
                if (!open)
                {
                    open = true;
                    StartCoroutine(AnimationUtilities.RotateLinear(true, this.gameObject, closeRot, openRot, 2, 0));
                }
                //else
                //{
                //    open = false;
                //    StartCoroutine(AnimationUtilities.RotateLinear(true, this.gameObject, openRot, closeRot, 2, 0));
                //}
            }
        }
        else if (Input.GetKeyUp("e"))
        {
            if (Agent && !open)
            {
                open = true;
                StartCoroutine(AnimationUtilities.RotateLinear(true, this.gameObject, closeRot, openRot, 2, 0));
            }
            //else
            //{
            //    open = false;
            //    StartCoroutine(AnimationUtilities.RotateLinear(true, this.gameObject, openRot, closeRot, 2, 0));
            //}
        }
    }
    public void TriggerOn(Player agent)
    {
        Agent = agent;
    }

    public void TriggerOff()
    {
        Agent = null;
    }

    public bool IsOpen()
    {
        return open;
    }
}
