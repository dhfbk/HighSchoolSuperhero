using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public GameObject dest;
    public GameObject[] dests;
    private GameObject player;
    public float speed;
    public int i = 0;
    private bool free;

    public float maximumDistance;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        //SinglePLayer only
        if (IsFree() == false)
        {
            if (Vector3.Distance(player.transform.position, this.transform.position) < maximumDistance)
            {
                if (dest != null)
                {
                    transform.position = Vector3.MoveTowards(transform.position, dest.transform.position, Time.deltaTime * speed);
                    if (Vector3.Distance(dest.transform.position, transform.position) < 0.75f)
                    {
                        if (i < dests.Length - 1)
                        {
                            i++;
                        }
                        else
                        {
                            i = 0;
                        }
                        dest = dests[i];
                        transform.LookAt(dest.transform.position);
                    }
                }
            }
        }
    }
    public void SetFree(bool free)
    {
        this.free = free;
    }
    public bool IsFree()
    {
        return free;
    }
}
