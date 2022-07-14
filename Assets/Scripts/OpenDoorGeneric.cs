using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OpenDoorGeneric : MonoBehaviour, ITriggerable
{
    public bool Raycasted { get; set; }
    public Player Agent { get; set; }
    Quaternion initialRotation;
    Quaternion destRotation;
    float t;
    bool open;

    //Dot product
    Vector3 playerDir;

    // Start is called before the first frame update
    void Start()
    {
        initialRotation = transform.rotation;
        destRotation = Quaternion.Euler(initialRotation.eulerAngles + new Vector3(0, -90, 0));
    }
    void Update()
    {
        //Opening animations
        if (open)
        {
            if (t <= 1f)
            {
                t += Time.deltaTime;
                transform.rotation = Quaternion.Lerp(transform.rotation, destRotation, t);
            }
        }
        else
        {
            if (t <= 1f)
            {
                t += Time.deltaTime;
                transform.rotation = Quaternion.Lerp(transform.rotation, initialRotation, t);
            }
        }
    }

    public void TriggerOn(Player agent)
    {
        t = 0;
        open = true;
        playerDir = Control.Instance.transform.position - transform.parent.position;

        //Is the player in front?
        if (Vector3.Dot(transform.parent.forward, playerDir) > 0)
            destRotation = Quaternion.Euler(initialRotation.eulerAngles + new Vector3(0, -90, 0));
        else
            destRotation = Quaternion.Euler(initialRotation.eulerAngles + new Vector3(0, 90, 0));
    }

    public void TriggerOff()
    {
        t = 0;
        open = false;
    }
}
