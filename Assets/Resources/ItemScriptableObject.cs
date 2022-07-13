using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
public class ItemScriptableObject : ScriptableObject
{
    public Sprite icon;
    public string name;
    public int amount;
    public int price;
    public string engDescription;
    public string itaDescription;
    public int level;

}
