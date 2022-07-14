using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour, ITriggerable
{
    public Player Agent { get; set; }
    public GameObject dest;

    private void Update()
    {
        if (Input.GetKeyUp("e") && Agent)
        {
            TeleportManager.Teleport(Agent, dest.transform.position);
        }
    }

    public void TriggerOn(Player agent)
    {
        Agent = agent;
        HintUtility.ShowHint(agent, "E", ML.systemMessages.openDoor, 5);
    }

    public void TriggerOff()
    {
        Agent = null;
        HintUtility.HideHint();
    }
}
