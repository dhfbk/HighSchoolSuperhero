using System;
using UnityEngine;
using TMPro;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
using System.Collections.Generic;

public interface ISlotData
{
    string[] Sprites { get; set; }
    int[] Amounts { get; set; }
}

[Serializable]
public class InvSData : ISlotData
{
    public string[] Sprites { get; set; }
    public int[] Amounts { get; set; }
}

[Serializable]
public class ItemData
{
    public int HP;
    public int EN;
    public int DMG;
    public int RNG;
    public string SP;
}
public class InventoryS : MonoBehaviour
{

    public GameObject selectedSlot;
    public MenuObject selectedItem;
    public MenuObject holdingItem;
    public int menuItemLayer;
    public int holdItemLayer;
    public int cloneItemLayer;
    int menuNumberLayer;
    int cloneNumberLayer;
    int holdNumberLayer;

    public static List<GameObject> slots;
    public static InvSData invData;
    public static Canvas canvas;
    //public ItemInfo itemInfo;

    public static List<GameObject> lootSlots;
    public static InvSData lootData;
    public static string lootName;

    public ItemData itemData;
    public TextAsset itemDataFile;

    public InvSData currentData;
    public int invIndex;

    public bool loadFromFile;
    void Start()
    {
        menuItemLayer = menuItemLayer == 0 ? 32002 : menuItemLayer;
        cloneItemLayer = cloneItemLayer == 0 ? 32004 : cloneItemLayer;
        holdItemLayer = holdItemLayer == 0 ? 32006 : holdItemLayer;
        menuNumberLayer = menuNumberLayer == 0 ? 32003 : menuNumberLayer;
        cloneNumberLayer = cloneNumberLayer == 0 ? 32005 : cloneNumberLayer;
        holdNumberLayer = holdNumberLayer == 0 ? 32007 : holdNumberLayer;

        canvas = transform.parent.GetComponent<Canvas>();
        invData = new InvSData();
        slots = new List<GameObject>();
        slots = GetChildren(transform);
        invData.Sprites = new string[slots.Count];
        invData.Amounts = new int[slots.Count];

        lootData = new InvSData();
        itemData = new ItemData();
        //Load
        string dir = Path.Combine(Application.persistentDataPath, "InventoryS.hs");
        if (File.Exists(dir) && loadFromFile)
        {
            //Distruggi tutto e carica da file
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].transform.childCount > 0)
                {
                    Destroy(slots[i].transform.GetChild(0).gameObject);
                }
            }
            InvSData serializableData = new InvSData();
            XmlSerializer serializer = new XmlSerializer(typeof(InvSData));
            FileStream stream = new FileStream(dir, FileMode.Open);
            serializableData = serializer.Deserialize(stream) as InvSData;
            invData = serializableData;
            stream.Close();
            LoadInventory(invData, slots);
        }
        else
        {
            //Usa oggetti dall'editor
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].transform.childCount > 0)
                {
                    invData.Sprites[i] = slots[i].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite.name;
                    invData.Amounts[i] = slots[i].transform.GetChild(0).GetComponent<MenuObject>().amount;
                }
                else
                {
                    invData.Sprites[i] = null;
                    invData.Amounts[i] = 0;
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            //Mouse su slot
            if (selectedSlot)
            {
                if (slots.Contains(selectedSlot))
                {
                    currentData = invData;
                    invIndex = slots.IndexOf(selectedSlot);
                }
                else
                {
                    //currentData = lootData;
                    //invIndex = lootSlots.IndexOf(selectedSlot);
                }
                //Mouse su oggetto
                if (selectedItem)
                {
                    //Oggetto tenuto
                    if (holdingItem)
                    {
                        if (Input.GetKey(KeyCode.LeftControl))
                        {
                            if (selectedItem.spriteR.sprite.name == holdingItem.spriteR.sprite.name)
                                CollectOne();
                            else
                                Subs();
                        }
                        else
                        {
                            if (selectedItem.spriteR.sprite.name == holdingItem.spriteR.sprite.name)
                                PlaceAll();
                            else
                                Subs();
                        }
                    }
                    else
                    {
                        if (Input.GetKey(KeyCode.LeftControl))
                            CollectOne();
                        else
                            CollectAll(); //Se non ho niente e clicco su un item
                    }
                }
                else
                {
                    if (holdingItem)
                        PlaceAll();
                }
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            if (selectedSlot)
            {
                if (slots.Contains(selectedSlot))
                {
                    currentData = invData;
                    invIndex = slots.IndexOf(selectedSlot);
                }
                else
                {
                    currentData = lootData;
                    invIndex = lootSlots.IndexOf(selectedSlot);
                }
                if (selectedItem)
                {
                    if (!holdingItem)
                    {
                        UseObject(selectedItem);
                    }
                    else
                    {
                        if (Input.GetKey(KeyCode.LeftControl))
                        {
                            if (selectedItem.spriteR.sprite.name == holdingItem.spriteR.sprite.name)
                                PlaceOne();
                            else
                                Subs();
                        }
                        else
                        {
                            UseObject(holdingItem);
                        }
                    }
                }
                else
                {
                    if (holdingItem)
                    {
                        if (Input.GetKey(KeyCode.LeftControl))
                            PlaceOne();
                    }
                }
            }
        }
    }

    void PlaceOne()
    {
        MenuObject menuItem = holdingItem.GetComponent<MenuObject>();
        if (menuItem.amount == 1)
        {
            PlaceAll();
        }
        else
        {
            menuItem.ChangeAmount(-1);
            Clone(holdingItem, false);
        }
    }

    void CollectOne()
    {
        if (selectedItem.amount == 1)
        {
            CollectAll();
        }
        else
        {
            Clone(selectedItem, true);
            selectedItem.ChangeAmount(-1);
        }
    }
    void CollectAll()
    {
        Get(selectedItem);
        if (holdingItem)
        {
            selectedItem.destroy = true;
            holdingItem.ChangeAmount(selectedItem.amount);
        }
        else
        {
            holdingItem = selectedItem;
        }
        currentData.Sprites[invIndex] = null;
        currentData.Amounts[invIndex] = 0;
        selectedItem = null;
    }

    void PlaceAll()
    {
        Drop(holdingItem);

        if (selectedItem)
        {
            currentData.Amounts[invIndex] += holdingItem.amount;
            selectedItem.ChangeAmount(holdingItem.amount);
            holdingItem.destroy = true;
            holdingItem = null;
        }
        else
        {
            currentData.Sprites[invIndex] = holdingItem.spriteR.sprite.name;
            currentData.Amounts[invIndex] = holdingItem.amount;
            selectedItem = holdingItem;
            holdingItem = null;
        }
    }

    void Subs()
    {
        MenuObject holdItem = holdingItem;
        Drop(holdItem);
        MenuObject slotItem = selectedItem;
        Get(slotItem);
        holdingItem = slotItem;
        selectedItem = holdItem;
        currentData.Sprites[invIndex] = holdItem.spriteR.sprite.name;
        currentData.Amounts[invIndex] = holdItem.amount;
        //itemInfo.UpdateDescription(slotItem);
    }

    void Clone(MenuObject item, bool collect)
    {
        GameObject clone = Instantiate(item.gameObject, item.transform.position, Quaternion.identity);
        clone.transform.name = "Sprite";
        clone.transform.SetParent(transform);
        clone.transform.localScale = item.transform.localScale;
        clone.transform.rotation = item.transform.rotation;

        MenuObject menuClone = clone.GetComponent<MenuObject>();
        menuClone.SetAmount(1);
        if (collect)
        {
            menuClone.gameObject.GetComponent<Renderer>().sortingOrder = holdItemLayer;
            menuClone.amountObj.GetComponent<Renderer>().sortingOrder = holdNumberLayer;
            menuClone.follow = true;
            if (holdingItem)
            {
                menuClone.destroy = true;
                holdingItem.ChangeAmount(1);
            }
            else
            {
                holdingItem = clone.GetComponent<MenuObject>();
            }
            currentData.Amounts[invIndex] -= 1;
        }
        else
        {
            menuClone.follow = false;
            menuClone.destSlot = selectedSlot.transform.position;
            menuClone.gameObject.GetComponent<Renderer>().sortingOrder = menuItemLayer;
            menuClone.amountObj.GetComponent<Renderer>().sortingOrder = menuNumberLayer;
            menuClone.transform.SetParent(selectedSlot.transform);
            if (selectedItem)
            {
                selectedItem.ChangeAmount(1);
                menuClone.destroy = true;
            }
            else
            {
                selectedItem = menuClone;
                currentData.Sprites[invIndex] = menuClone.spriteR.sprite.name;
            }
            currentData.Amounts[invIndex] += 1;
        }
    }

    void Drop(MenuObject holdItem)
    {
        holdItem.follow = false;
        holdItem.startPos = holdItem.transform.position;
        holdItem.destSlot = selectedSlot.transform.position;
        holdItem.gameObject.GetComponent<Renderer>().sortingOrder = menuItemLayer;
        holdItem.amountObj.GetComponent<Renderer>().sortingOrder = menuNumberLayer;
        holdItem.transform.SetParent(selectedSlot.transform);
    }

    void Get(MenuObject slotItem)
    {
        slotItem.transform.SetParent(transform);
        slotItem.gameObject.GetComponent<Renderer>().sortingOrder = holdItemLayer;
        slotItem.amountObj.GetComponent<Renderer>().sortingOrder = holdNumberLayer;
        slotItem.startPos = slotItem.transform.position;
        slotItem.follow = true;
    }

    public static void LoadInventory(InvSData data, List<GameObject> slots)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (data.Amounts[i] > 0)
            {
                if (slots[i].transform.childCount == 0)
                {
                    GameObject sp = Instantiate(Resources.Load<GameObject>("MenuSpritePrefab"));
                    sp.transform.SetParent(slots[i].transform);
                    sp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Items/" + data.Sprites[i].Trim());
                    sp.name = "Sprite";
                    sp.transform.position = sp.transform.parent.position;
                    MenuObject spItem = sp.GetComponent<MenuObject>();
                    spItem.amount = data.Amounts[i];
                    spItem.instantiated = true;
                    spItem.canvas = canvas;
                    spItem.destSlot = slots[i].transform.position;
                    spItem.startScale = new Vector3(0.9f, 0.9f, 0.9f);
                }
                else
                {
                    slots[i].transform.GetChild(0).GetComponent<MenuObject>().SetAmount(data.Amounts[i]);

                    //Resetta posizioni e scale degli oggetti se ci sono animazioni in sospeso
                    slots[i].transform.GetChild(0).localScale = new Vector3(0.9f, 0.9f, 0.9f);
                    slots[i].transform.GetChild(0).localPosition = Vector3.zero;
                }
            }
            else
            {
                if (data.Sprites[i] != null)
                {
                    Destroy(slots[i].transform.GetChild(0).gameObject);
                    data.Sprites[i] = null;
                }
            }
        }
    }
    public static void SaveInventory()
    {
        InvSData serializableData = invData;
        XmlSerializer serializer = new XmlSerializer(typeof(InvSData));
        FileStream stream = new FileStream(Path.Combine(Application.persistentDataPath, "InventoryS.hs"), FileMode.Create);
        serializer.Serialize(stream, serializableData);
        stream.Close();
    }

    void UseObject(MenuObject item)
    {
    }

    public static List<GameObject> GetChildren(Transform t)
    {
        List<GameObject> children;
        children = new List<GameObject>();
        foreach (Transform child in t)
        {
            if (child.CompareTag("SlotS"))
            {
                children.Add(child.gameObject);
            }
        }
        return children;
    }
    void OnApplicationQuit()
    {
        SaveInventory();
    }
}
