using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketShoesItem : MonoBehaviour, IItem
{
    public string Name { get => "Rocket Shoes"; }
    public int DefaultAmount { get => 1; }
    public int DefaultPrice { get => 150; }
    public string Description { get => "Put these on to jump really high. You cannot fly though (yet!)."; }
    public void Purchase(Player agent)
    {
        if (!agent.ScarpeMolla)
        {
            if (agent.GetCrystals() >= DefaultPrice)
            {
                agent.ScarpeMolla = true;
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
