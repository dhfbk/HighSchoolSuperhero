using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToScientisit : MonoBehaviour
{
    bool gone;
    Player Agent;
    Vector3 agentEnd;
    public GameObject end;


    // Update is called once per frame
    void Update()
    {
        if (!gone && GetComponent<Pan>().State == "done")
        {
            Agent = GetComponent<Pan>().Agent;
            agentEnd = end.transform.position;
            Invoke("Done", 3);
            gone = true;
        }
    }

    void Done()
    {
        TeleportManager.Teleport(Agent, agentEnd, true);
        Agent.transform.LookAt(GetComponent<Pan>().target.transform);
        ScientistInteraction.scientistAutoStart = true;
    }
}
