//using System;
//using UnityEngine;
//using TMPro;
//using System.IO;
//using System.Xml.Serialization;
//using System.Linq;
//using System.Collections.Generic;

//[Serializable]
//public class InvSData
//{
//    public string[] sprites;
//    public int[] amounts;
//}

//[Serializable]
//public class LootSData
//{
//    static string[] sprites;
//    static int[] amounts;
//}
//public class Inventory : MonoBehaviour
//{
//    public Camera cam;

//    public GameObject selectedSlot;
//    public GameObject selectedItem;
//    public GameObject holdingItem;
//    public int menuItemLayer;
//    public int holdItemLayer;
//    public int cloneItemLayer;
//    int menuNumberLayer;
//    int cloneNumberLayer;
//    int holdNumberLayer;

//    public List<GameObject> slots;
//    public InvSData invData;
//    public Canvas canvas;
//    //public ItemInfo itemInfo;

//    void Start()
//    {
//        slots = new List<GameObject>();
//        slots = GetChildren();

//        menuItemLayer = menuItemLayer == 0 ? 32002 : menuItemLayer;
//        cloneItemLayer = cloneItemLayer == 0 ? 32004 : cloneItemLayer;
//        holdItemLayer = holdItemLayer == 0 ? 32006 : holdItemLayer;
//        menuNumberLayer = menuNumberLayer == 0 ? 32003 : menuNumberLayer;
//        cloneNumberLayer = cloneNumberLayer == 0 ? 32005 : cloneNumberLayer;
//        holdNumberLayer = holdNumberLayer == 0 ? 32007 : holdNumberLayer;
//        invData = new InvSData();
//        invData.sprites = new string[slots.Count];
//        invData.amounts = new int[slots.Count];

//        //Load
//        LoadInventory(true);
//    }

//    void Update()
//    {
//        if (Input.GetMouseButtonUp(0))
//        {
//            //SaveInventory(true);

//            //Mouse su slot
//            if (selectedSlot)
//            {
//                //Mouse su oggetto
//                if (selectedItem)
//                {
//                    //Oggetto tenuto
//                    if (holdingItem)
//                    {
//                        if (Input.GetKey(KeyCode.LeftControl))
//                        {
//                            if (selectedItem.GetComponent<MenuObject>().spriteR.sprite.name == holdingItem.GetComponent<MenuObject>().spriteR.sprite.name)
//                            {
//                                CollectOne();
//                            }
//                            else
//                            {
//                                Subs();
//                            }
//                        }
//                        else
//                        {
//                            if (selectedItem.GetComponent<MenuObject>().spriteR.sprite.name == holdingItem.GetComponent<MenuObject>().spriteR.sprite.name)
//                            {

//                                PlaceAll();
//                            }
//                            else
//                            {
//                                Subs();
//                            }
//                        }
//                    }
//                    else
//                    {
//                        if (Input.GetKey(KeyCode.LeftControl))
//                        {
//                            CollectOne();
//                        }
//                        else
//                        {
//                            CollectAll();
//                        }
//                    }
//                }
//                else
//                {
//                    if (holdingItem)
//                    {
//                        PlaceAll();
//                    }
//                }
//            }
//        }
//        else if (Input.GetMouseButtonUp(1))
//        {
//            if (selectedSlot)
//            {
//                if (selectedItem)
//                {
//                    if (!holdingItem)
//                    {
//                        UseObject(selectedItem.GetComponent<MenuObject>());
//                    }
//                    else
//                    {
//                        if (Input.GetKey(KeyCode.LeftControl))
//                        {
//                            if (selectedItem.GetComponent<MenuObject>().spriteR.sprite.name == holdingItem.GetComponent<MenuObject>().spriteR.sprite.name)
//                            {
//                                PlaceOne();
//                            }
//                            else
//                            {
//                                Subs();
//                            }
//                        }
//                        else
//                        {
//                            UseObject(holdingItem.GetComponent<MenuObject>());
//                        }
//                    }
//                }
//                else
//                {
//                    if (holdingItem)
//                    {
//                        if (Input.GetKey(KeyCode.LeftControl))
//                        {
//                            PlaceOne();
//                        }
//                    }
//                }
//            }
//        }
//    }

//    void PlaceOne()
//    {
//        MenuObject menuItem = holdingItem.GetComponent<MenuObject>();
//        if (menuItem.amount == 1)
//        {
//            PlaceAll();
//        }
//        else
//        {
//            menuItem.ChangeAmount(-1);
//            Clone(holdingItem, false);
//        }
//    }

//    void CollectOne()
//    {
//        MenuObject menuItem = selectedItem.GetComponent<MenuObject>();
//        if (menuItem.amount == 1)
//        {
//            CollectAll();
//        }
//        else
//        {
//            Clone(selectedItem, true);
//            menuItem.ChangeAmount(-1);
//        }
//    }
//    void CollectAll()
//    {
//        MenuObject slotItem = selectedItem.GetComponent<MenuObject>();
//        Get(slotItem);
//        if (holdingItem)
//        {
//            slotItem.destroy = true;
//            holdingItem.GetComponent<MenuObject>().ChangeAmount(slotItem.amount);

//        }
//        else
//        {
//            holdingItem = selectedItem;
//        }
//        int i = slots.IndexOf(selectedSlot);
//        invData.sprites[i] = null;
//        invData.amounts[i] = 0;
//        selectedItem = null;
//        SaveInventory(false);

//    }

//    void PlaceAll()
//    {
//        MenuObject holdItem = holdingItem.GetComponent<MenuObject>();
//        Drop(holdItem);

//        int i = slots.IndexOf(selectedSlot);

//        if (selectedItem)
//        {
//            invData.amounts[i] += holdItem.amount;
//            selectedItem.GetComponent<MenuObject>().ChangeAmount(holdItem.amount);
//            holdItem.destroy = true;
//        }
//        else
//        {
//            invData.sprites[i] = holdItem.spriteR.sprite.name;
//            invData.amounts[i] = holdItem.amount;
//            selectedItem = holdItem.gameObject;
//            holdingItem = null;
//        }
//        SaveInventory(false);
//    }

//    void Subs()
//    {
//        MenuObject holdItem = holdingItem.GetComponent<MenuObject>();
//        Drop(holdItem);
//        MenuObject slotItem = selectedItem.GetComponent<MenuObject>();
//        Get(slotItem);
//        holdingItem = slotItem.gameObject;
//        selectedItem = holdItem.gameObject;

//        int i = slots.IndexOf(selectedSlot);
//        invData.sprites[i] = holdItem.spriteR.sprite.name;
//        invData.amounts[i] = holdItem.amount;
//        SaveInventory(false);
//        //itemInfo.UpdateDescription(slotItem);
//    }

//    void Clone(GameObject item, bool collect)
//    {
//        GameObject clone = Instantiate(item);
//        clone.transform.name = "Object";
//        clone.transform.position = item.transform.position;
//        clone.transform.rotation = item.transform.rotation;
//        clone.transform.SetParent(transform);

//        MenuObject menuClone = clone.GetComponent<MenuObject>();
//        menuClone.SetAmount(1);
//        if (collect)
//        {
//            clone.transform.localScale = item.transform.localScale * 2;
//            menuClone.gameObject.GetComponent<Renderer>().sortingOrder = holdItemLayer;
//            menuClone.amountObj.GetComponent<Renderer>().sortingOrder = holdNumberLayer;
//            menuClone.Follow();
//            if (holdingItem)
//            {
//                menuClone.destroy = true;
//                holdingItem.GetComponent<MenuObject>().ChangeAmount(1);
//            }
//            else
//            {
//                holdingItem = clone;
//            }

//            int i = slots.IndexOf(selectedSlot);
//            invData.amounts[i] -= 1;
//        }
//        else
//        {
//            clone.transform.localScale = item.transform.localScale;
//            menuClone.Unfollow();
//            menuClone.gameObject.GetComponent<Renderer>().sortingOrder = menuItemLayer;
//            menuClone.amountObj.GetComponent<Renderer>().sortingOrder = menuNumberLayer;
//            menuClone.transform.SetParent(selectedSlot.transform);
//            menuClone.destSlot = Vector3.zero;
//            if (selectedItem)
//            {
//                selectedItem.GetComponent<MenuObject>().ChangeAmount(1);
//                menuClone.destroy = true;
//            }
//            else
//            {
//                selectedItem = menuClone.gameObject;
//            }

//            int i = slots.IndexOf(selectedSlot);
//            invData.amounts[i] += 1;
//        }
//        SaveInventory(false);
//    }

//    void Drop(MenuObject holdItem)
//    {
//        holdItem.Unfollow();
//        holdItem.startPos = holdItem.transform.position;
//        //holdItem.destSlot = selectedSlot.transform.position;
//        holdItem.gameObject.GetComponent<Renderer>().sortingOrder = menuItemLayer;
//        holdItem.amountObj.GetComponent<Renderer>().sortingOrder = menuNumberLayer;
//        holdItem.transform.SetParent(selectedSlot.transform);
//        holdItem.destSlot = Vector3.zero;
//    }

//    void Get(MenuObject slotItem)
//    {
//        slotItem.transform.SetParent(transform);
//        slotItem.gameObject.GetComponent<Renderer>().sortingOrder = holdItemLayer;
//        slotItem.amountObj.GetComponent<Renderer>().sortingOrder = holdNumberLayer;
//        slotItem.startPos = slotItem.transform.position;
//        slotItem.Follow();


//    }

//    void UseObject(MenuObject item)
//    {

//    }

//    public void LoadInventory(bool file)
//    {
//        if (file)
//        {
//            XmlSerializer serializer = new XmlSerializer(typeof(InvSData));
//            FileStream stream = new FileStream(Path.Combine(Application.persistentDataPath, "Inventory.hs"), FileMode.Open);
//            invData = serializer.Deserialize(stream) as InvSData;
//            stream.Close();
//        }
//        for (int i = 0; i < slots.Count; i++)
//        {
//            if (invData.amounts[i] > 0)
//            {
//                if (slots[i].transform.childCount == 0)
//                {
//                    GameObject sp = Instantiate(Resources.Load<GameObject>("MenuObject"));
//                    sp.transform.SetParent(slots[i].transform);
//                    sp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Items/" + invData.sprites[i].Trim());
//                    sp.transform.position = sp.transform.parent.position;
//                    sp.transform.rotation = sp.transform.parent.rotation;
//                    sp.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
//                    sp.GetComponent<MenuObject>().destSlot = Vector3.zero;
//                    sp.GetComponent<MenuObject>().amount = invData.amounts[i];
//                    sp.GetComponent<MenuObject>().instantiated = true;
//                    sp.name = "Object";
//                    sp.GetComponent<MenuObject>().canvas = canvas;
//                }
//                else
//                {
//                    slots[i].transform.GetChild(0).GetComponent<MenuObject>().SetAmount(invData.amounts[i]);
//                }
//            }
//            else
//            {
//                if (invData.sprites[i] != null)
//                {
//                    Destroy(slots[i].transform.GetChild(0).gameObject);
//                    invData.sprites[i] = null;
//                }
//            }
//        }
//    }
//    public void SaveInventory(bool file)
//    {
//        for (int i = 0; i < slots.Count; i++)
//        {
//            if (slots[i].transform.childCount == 0)
//            {
//                invData.sprites[i] = null;
//                invData.amounts[i] = 0;
//            }
//        }
//        if (file)
//        {
//            XmlSerializer serializer = new XmlSerializer(typeof(InvSData));
//            FileStream stream = new FileStream(Path.Combine(Application.persistentDataPath, "Inventory.hs"), FileMode.Create);
//            serializer.Serialize(stream, invData);
//            stream.Close();
//        }
//    }

//    List<GameObject> GetChildren()
//    {
//        List<GameObject> children;
//        children = new List<GameObject>();
//        foreach (Transform child in transform)
//        {
//            if (child.CompareTag("Slot"))
//            {
//                children.Add(child.gameObject);
//            }
//        }
//        return children;
//    }

//    void OnApplicationQuit()
//    {
//        SaveInventory(true);
//    }
//}
