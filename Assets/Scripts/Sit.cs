using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sit : MonoBehaviour
{
    public bool sitting;
    public bool up;
    Vector3 dest;
    public bool gettingUp;
    Player agent;
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            agent = GetComponent<ITriggerable>().Agent;
            if (GetComponent<MouseOver>().on == true)
            {
                if (!sitting)
                {
                    Prepare(agent, true);
                }
            }
            else
            {
                if (sitting)
                {
                    Prepare(agent, false);
                }
            }
        }

        if (sitting)
        {
            if (up)
            {
                agent.transform.position = Vector3.MoveTowards(agent.transform.position, dest, Time.deltaTime * 5);
                if (agent.transform.position == dest)
                {
                    up = false;
                    dest = new Vector3(transform.position.x, transform.position.y + 0.75F, transform.position.z + 0.2F);
                }
            }
            else
            {
                agent.transform.position = Vector3.MoveTowards(agent.transform.position, dest, Time.deltaTime * 5);
            }
            agent.transform.LookAt(new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z - 1));
        }
        //GetUp
        else if (gettingUp)
        {
            if (up)
            {
                agent.transform.position = Vector3.MoveTowards(agent.transform.position, dest, Time.deltaTime * 5);
                if (agent.transform.position == dest)
                {
                    up = false;
                    dest = new Vector3(transform.position.x + 1, transform.position.y + 0.2f, transform.position.z);
                }
            }
            else
            {
                agent.transform.position = Vector3.MoveTowards(agent.transform.position, dest, Time.deltaTime * 5);
                if (agent.transform.position == dest)
                {
                    gettingUp = false;
                    agent.GetComponent<Movement>().enabled = true;
                    agent.GetComponent<Rigidbody>().useGravity = true;
                    agent.GetComponent<CapsuleCollider>().enabled = true;
                    agent.GetComponent<Animator>().SetBool("GetUp", false);
                }
            }
        }
    }

    void Prepare(Player agent, bool sit)
    {
        if (sit)
        {
            agent.GetComponent<Movement>().enabled = false;
            agent.GetComponent<Animator>().SetBool("Sit", true);
            agent.GetComponent<Animator>().SetBool("GetUp", false);
            agent.GetComponent<Rigidbody>().useGravity = false;
            agent.GetComponent<CapsuleCollider>().enabled = false;
            dest = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z + 0.2F);
            sitting = true;
            gettingUp = false;
            up = true;
        }
        else
        {
            agent.transform.eulerAngles = new Vector3(0, 90, 0);
            agent.GetComponent<Animator>().SetBool("GetUp", true);
            agent.GetComponent<Animator>().SetBool("Sit", false);
            dest = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
            sitting = false;
            gettingUp = true;
            up = true;
        }
    }
}
