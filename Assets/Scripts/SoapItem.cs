using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem
{
    string Name { get;}
    int DefaultAmount { get;}
    int DefaultPrice { get; }
    string Description { get; }
    void Purchase(Player agent);
}
public class SoapItem : MonoBehaviour, IItem
{
    public string Name { get => "Soap"; }
    public int DefaultAmount { get => 50; }
    public int DefaultPrice { get => 50; }
    public string Description { get => "A special type of soap. Use it to erase the graffiti."; }
    public void Purchase(Player agent)
    {
        if (agent.GetSoap() < agent.GetMaxSoap())
        {
            if (agent.GetCrystals() >= DefaultPrice)
            {
                agent.AddSoap(DefaultAmount);
                agent.AddCrystals(-DefaultPrice);
            }
            else
                NotificationUtility.ShowString(agent, "Not enough crystals!");
        }
        else
            NotificationUtility.ShowString(agent, "Already full!");
    }
}
