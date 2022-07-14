using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface ITriggerable
{
    Player Agent { get; set; }
    void TriggerOn(Player agent);
    void TriggerOff();
}
public class GenericTrigger : MonoBehaviour, ITriggerable
{
    public bool Raycasted { get; set; }
    public Player Agent { get; set; }
    public void TriggerOn(Player agent)
    {
        this.Agent = agent;
    }
    public void TriggerOff()
    {
        this.Agent = null;
    }
}
