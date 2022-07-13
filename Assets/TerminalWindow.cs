using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalWindow : MonoBehaviour
{
    public GameObject itemContainer;
    public ItemScriptableObject[] itemSOs;
    public ItemScriptableObject[] NoRestrictionItemSOs;
    public GameObject buttonPrefab;
    // Start is called before the first frame update
    void Start()
    {
        GenerateItems();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateItems()
    {
        if (itemContainer.transform.childCount > 0)
        {
            foreach(Transform t in itemContainer.transform)
            {
                Destroy(t.transform.gameObject);
            }
        }

        ItemScriptableObject[] items;
        if (Player.rCondition == RCondition.NonRestricted)
            items = NoRestrictionItemSOs;
        else
            items = itemSOs;

        foreach (ItemScriptableObject iso in items)
        {
            GameObject b = Instantiate(buttonPrefab);
            b.transform.parent = itemContainer.transform;
            b.GetComponent<TerminalButton>().itemSO = iso;
            b.transform.localPosition = new Vector3(b.transform.localPosition.x, b.transform.localPosition.y, 0);
            b.transform.localScale = new Vector2(5, 5);
            b.transform.rotation = b.transform.parent.rotation;
        }
    }
}
