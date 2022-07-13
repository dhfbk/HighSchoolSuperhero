using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNPC : MonoBehaviour
{
    public int spawnNumber;
    public GameObject[] dests;
    public float speed;
    private int destIndex;
    public int DestIndex { get => destIndex;
        set
        {
            if (value > dests.Length - 1)
            {
                destIndex = 0;
            }
            else
            {
                destIndex = value;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (speed == 0F)
            speed = 3F;
        if (spawnNumber > dests.Length)
        {
            spawnNumber = dests.Length;
        }
        else
        {
            //Create many npcs
            for (int i = 0; i < spawnNumber; i++)
            {
                DestIndex = i;
                GameObject npc = NPCUtilities.CreateNPC(random:true, pos:dests[DestIndex].transform.position, free:false);
                
                npc.transform.SetParent(this.transform);
                NPCController npcc = npc.GetOrAddComponent<NPCController>();
                Destroy(npc.GetComponent<GoBackToPlace>());
                DestIndex += 1;
                npc.transform.LookAt(dests[DestIndex].transform.position);
                npcc.dest = dests[DestIndex];
                npcc.dests = dests;
                npcc.speed = speed;
                npc.GetComponent<Animator>().SetBool("Walk", true);
                npcc.i = DestIndex;
            }
        }
    }
}
