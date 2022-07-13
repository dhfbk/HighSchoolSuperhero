using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GliderItem : MonoBehaviour, IItem
{
    public string Name { get => "Glider"; }
    public int DefaultAmount { get => 1; }
    public int DefaultPrice { get => 200; }
    public string Description { get => "Use this glider to jump off buildings with grace (and without smashing yourself into the sidewalk)."; }
    public void Purchase(Player agent)
    {
        if (!agent.Glider)
        {
            if (agent.GetCrystals() >= DefaultPrice)
            {
                agent.Glider = true;
                agent.AddCrystals(-DefaultPrice);
                Destroy(this.gameObject);
            }
            else
                NotificationUtility.ShowString(agent, "Not enough crystals!");
        }
        else
            NotificationUtility.ShowString(agent, "Already full!");
    }
}
