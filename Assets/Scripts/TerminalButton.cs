using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TerminalButton : MonoBehaviour
{
    public Image icon;
    public int amount;
    public TextMeshProUGUI priceT;
    public TextMeshProUGUI description;
    public ItemScriptableObject itemSO;
    public TextMeshProUGUI name;

    public void Start()
    {
        Player agent = transform.root.GetComponent<FollowPlayer>().target.GetComponent<Player>();
        amount = itemSO.amount;
        priceT.text = itemSO.price.ToString();
        if (Player.language == ML.Lang.en)
            description.text = itemSO.engDescription;
        else if (Player.language == ML.Lang.it)
            description.text = itemSO.itaDescription;
        name.text = itemSO.name;
        icon.sprite = itemSO.icon;
    }
    public void Click()
    {
        int price = int.Parse(priceT.text);
        Player agent = transform.root.GetComponent<FollowPlayer>().target.GetComponent<Player>();
        Purchase(agent);
    }

    public void Purchase(Player agent)
    {
        if (agent.GetLevel() >= itemSO.level)
        {
            if (agent.GetCrystals() >= itemSO.price)
            {
                if (itemSO.name.Contains("Soap") && agent.GetSoap() < agent.GetMaxSoap())
                {
                    agent.AddSoap(amount);
                    agent.AddCrystals(-itemSO.price);
                }
                else if (itemSO.name.Contains("Energy") && agent.GetEnergy() < agent.GetMaxEnergy())
                {
                    agent.AddEnergy(amount);
                    agent.AddCrystals(-itemSO.price);
                }
                else if (itemSO.name == "Rocket" && !agent.ScarpeMolla)
                {
                    agent.SetRocket(true);
                    agent.AddCrystals(-itemSO.price);
                }
                else if (itemSO.name == "Glider" && !agent.Glider)
                {
                    agent.SetGlider(true);
                    agent.AddCrystals(-itemSO.price);
                }
                else
                {
                    NotificationUtility.ShowString(agent, "Already full!");
                }
            }
            else
                NotificationUtility.ShowString(agent, "Not enough crystals!");
        }
        else
            NotificationUtility.ShowString(agent, $"Level up first! Level {itemSO.level} required.");
    }
}
