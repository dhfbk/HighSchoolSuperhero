using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSlot : MonoBehaviour
{
    //ItemInfo itemInfo;
    InventoryS invS;
    // Start is called before the first frame update
    void Start()
    {
        invS = Control.Instance.inv.GetComponent<InventoryS>();
        //itemInfo = inv.itemInfo;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnMouseEnter()
    {
        print(this.gameObject.name);
        invS.selectedSlot = this.gameObject;
        if (transform.childCount > 0)
        {
            invS.selectedItem = transform.GetChild(0).gameObject.GetComponent<MenuObject>();
            //itemInfo.ShowDescription(invS.selectedItem.GetComponent<MenuObject>());

        }
        else
        {
            invS.selectedItem = null;
        }
    }
    void OnMouseExit()
    {
        //itemInfo.DeleteEverythingIfNotHolding();
        invS.selectedSlot = null;
        invS.selectedItem = null;
    }
}
