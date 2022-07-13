using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyItem : MonoBehaviour, IItem
{
    public string Name { get => "Energy"; }
    public int DefaultAmount { get => 1; }
    public int DefaultPrice { get => 25; }
    public string Description { get => "Recharge your device by 1 notch."; }
    public void Purchase(Player agent)
    {
        if (agent.GetEnergy() < 7)
        {
            if (agent.GetCrystals() >= DefaultPrice)
            {
                agent.AddEnergy(DefaultAmount);
                agent.AddCrystals(-DefaultPrice);
            }
            else
                NotificationUtility.ShowString(agent, "Not enough crystals!");
        }
        else
            NotificationUtility.ShowString(agent, "Already full!");
    }
}
