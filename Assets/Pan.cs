using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour, ISaveable
{
    public GameObject target;
    public string State { get; set; }

    public Player Agent;
    bool busyAtTheEnd;
    private void OnTriggerEnter(Collider other)
    {
        if (State != "done")
        {
            if (other.GetComponent<Player>())
            {
                Agent = other.GetComponent<Player>();
                Agent.cameraInterface.cameraOrbit.CameraPan(target, 3);
                State = "done";
                SaveManager.SaveState(Agent, new ObjectState(this.gameObject, false));
            }
        }
    }

}
